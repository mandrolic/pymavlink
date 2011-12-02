using System;
using System.IO;
using System.Threading;
#if MF_FRAMEWORK
using Microsoft.SPOT;
#endif

namespace MavLink
{
    /// <summary>
    /// The array of bytes that was received on the Mavlink link
    /// </summary>
    public class PacketDecodedEventArgs : EventArgs
    {
        ///<summary>
        ///</summary>
        public PacketDecodedEventArgs(byte[] payload, byte sequenceNumber)
        {
            Payload = payload;
            SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// The packet payload - ie. the packet starting at SystemID and ending at
        /// the end of the packet data
        /// </summary>
        public readonly byte[] Payload;

        /// <summary>
        /// The sequence number that the packet had (rolling incremented byte)
        /// </summary>
        public byte SequenceNumber;
    }


   ///<summary>
   /// Handler for an PacketDecoded Event
   ///</summary>
   public delegate void PacketDecodedEventHandler(object sender, PacketDecodedEventArgs e);


   ///<summary>
   /// Describes an occurance when a packet fails CRC
   ///</summary>
   public class PacketCRCFailEventArgs : EventArgs
   {
       ///<summary>
       ///</summary>
       public PacketCRCFailEventArgs(byte[] badPacket)
       {
           BadPacket = badPacket;
       }

       /// <summary>
       /// The bytes that filed the CRC, including the starting character
       /// </summary>
       public byte[] BadPacket;
   }

   ///<summary>
   /// Handler for an PacketFailedCRC Event
   ///</summary>
   public delegate void PacketCRCFailEventHandler(object sender, PacketCRCFailEventArgs e);


/// <summary>
/// This can send and receive bytes over a mavlink connection. Escaping, CRC etc are all done here
/// </summary>
   public interface IDataLink
   {
       event PacketDecodedEventHandler PacketDecoded;
       void SendPacket(byte[] packetData);
   }

   ///<summary>
   /// 
   ///</summary>
   public class Mavlink_Link : IDataLink
    {
       private readonly Stream _ioStream;
       //private volatile bool stopThread;
       private bool stopThread; // no volatile in uFramework
       private byte[] _leftovers;

        public UInt32 PacketsReceived { get; private set; }
        public UInt32 BadCrcPacketsReceived { get; private set; }

        public event PacketDecodedEventHandler PacketDecoded;
        public event PacketCRCFailEventHandler PacketFailedCRC;
        public event PacketCRCFailEventHandler BytesUnused;


        public byte txPacketSequence; // public so it can be manipulated for testing
        private readonly Thread _receiveThread;

       public Mavlink_Link(Stream stream)
       {
           _ioStream = stream;
           _receiveThread = new Thread(ReceiveBytes);
           _leftovers = new byte[] {};
       }


       public void Start()
       {
           _receiveThread.Start();
       }

       public void Stop()
       {
           stopThread = true;
           var timeoutMs = 500;
           if (!_receiveThread.Join(timeoutMs))
               _receiveThread.Abort();
       }

       public void ReceiveBytes()
        {
            byte[] inbuf;

            while (stopThread == false)
            {
                inbuf = new byte[4];

                int numBytesRead;
                try
                {
                    numBytesRead = _ioStream.Read(inbuf, 0, inbuf.Length);
                }
                catch(Exception e) // todo: what is this exception?
                {
                    break;
                }

                var processBuf = new byte[numBytesRead];

                Array.Copy(inbuf, processBuf, numBytesRead);

                AddReadBytes(processBuf);
            }
        }

        public void SendPacket(byte[] packetData)
        {
            /*
               * Byte order:
               * 
               *  Packet start sign	 0x55, ASCII: U
               * 1	 Payload length	 0 - 255
               * 2	 Packet sequence	 0 - 255
               * 3	 System ID	 1 - 255
               * 4	 Component ID	 0 - 255
               * 5	 Message ID	 0 - 255
               * 6 to (n+6)	 Data	 (0 - 255) bytes
               * (n+7) to (n+8)	 Checksum (high byte, low byte)
               *
               */
            var outBytes = new byte[packetData.Length + 5];

            outBytes[0] = 0x55;
            outBytes[1] = (byte)(packetData.Length-3);  // 3 bytes for sequence, id, msg type which this 
                                                        // layer does not concern itself with
            outBytes[2] = txPacketSequence;

            txPacketSequence = unchecked(txPacketSequence++);

            int i;

            for ( i = 0; i < packetData.Length; i++)
            {
                outBytes[i + 3] = packetData[i];
            }

            // Check the CRC. Does not include the starting 'U' byte but does include the length
            var crc1 = Mavlink_Crc.Calculate(outBytes, 1, (UInt16)(packetData.Length + 2));

            byte crc_high = (byte)(crc1 & 0xFF);
            byte crc_low = (byte)(crc1 >> 8);

            outBytes[i + 3] = crc_high;
            outBytes[i + 4] = crc_low;

            _ioStream.Write(outBytes, 0, outBytes.Length);
        }


        /// <summary>
        /// Process latest bytes from the stream
        /// </summary>
        public void AddReadBytes(byte[] newlyReceived)
        {
            uint i = 0;

            // copy the old and new into a contiguous array
            // This is pretty inefficient...
            var bytesToProcess = new byte[newlyReceived.Length + _leftovers.Length];
            int j = 0;

            for (i = 0; i < _leftovers.Length; i++)
                bytesToProcess[j++] = _leftovers[i];

            for (i = 0; i < newlyReceived.Length; i++)
                bytesToProcess[j++] = newlyReceived[i];

            i = 0;

            // we are going to loop and decode packets until we use up the data
            // at which point we will return. Hence one call to this method could
            // result in multiple packet decode events
            while (true)
            {
                // Hunt for the start char
                while (i < bytesToProcess.Length && bytesToProcess[i] != 0x55)
                    i++;

                if (i == bytesToProcess.Length)
                {
                    // No start byte found in all our bytes. Dump them, Exit.
                    _leftovers = new byte[] { };
                    return;
                }
                
                if (i > 0)
                {
                    // if we get here then are some bytes which this code thinks are 
                    // not interesting and would be dumped. For diagnostics purposes,
                    // lets pop these bytes up in an event.
                    // Todo: this event is not necessary for comms. Surround with some sort of #ifdef

                    var badBytes = new byte[i];
                    Array.Copy(bytesToProcess, badBytes, (int)i);

                    if (BytesUnused!=null)
                        BytesUnused(this, new PacketCRCFailEventArgs(badBytes));
                }

                // We need at least the minimum length of a packet to process it. 
                // The minimum packet length is 8 bytes for acknowledgement packets without payload
                // if we don't have the minimum now, go round again
                if (bytesToProcess.Length - i < 8)
                {
                    // The minimum packet length is 8 bytes for acknowledgement packets without payload
                    _leftovers = new byte[bytesToProcess.Length - i];
                    j = 0;
                    while (i < bytesToProcess.Length)
                        _leftovers[j++] = bytesToProcess[i++];
                    return;
                }

                /*
                 * Byte order:
                 * 
                 *  Packet start sign	 0x55, ASCII: U
                 * 1	 Payload length	 0 - 255
                 * 2	 Packet sequence	 0 - 255
                 * 3	 System ID	 1 - 255
                 * 4	 Component ID	 0 - 255
                 * 5	 Message ID	 0 - 255
                 * 6 to (n+6)	 Data	 (0 - 255) bytes
                 * (n+7) to (n+8)	 Checksum (high byte, low byte)
                 *
                 */
                UInt16 payLoadLength = bytesToProcess[i + 1];

                // Now we know the packet length, 
                // If we don't have enough bytes in this packet to satisfy that packet lenghth,
                // then dump the whole lot in the leftovers and do nothing else - go round again
                if (payLoadLength > (bytesToProcess.Length - i - 8)) // payload + 'overhead' bytes (crc, system etc)
                {
                    // back up to the start char for next cycle
                    j = 0;

                    _leftovers = new byte[bytesToProcess.Length - i];

                    for (; i < bytesToProcess.Length; i++)
                    {
                        _leftovers[j++] = bytesToProcess[i];
                    }
                    return;
                }

                i++;

                // Check the CRC. Does not include the starting 'U' byte but does include the length
                var crc1 = Mavlink_Crc.Calculate(bytesToProcess, (UInt16)(i), (UInt16)(payLoadLength + 5));

                byte crc_high = (byte)(crc1 & 0xFF);
                byte crc_low = (byte)(crc1 >> 8);

                if (bytesToProcess[i + 5 + payLoadLength] == crc_high && bytesToProcess[i + 6 + payLoadLength] == crc_low)
                {
                    // This is used for data drop outs metrics, not packet windows
                    // so we should consider this here. 
                    // We pass up to subscribers only as an advisory thing
                    var rxPacketSequence = bytesToProcess[++i];
                    i++;
                    var packet = new byte[payLoadLength + 3];  // +3 because we are going to send up the sys and comp id and msg type with the data

                    for (j = 0; j < packet.Length; j++)
                        packet[j] = bytesToProcess[i + j];

                    OnPacketDecoded(packet, rxPacketSequence);
                   
                    //  advance i here by j to avoid unecessary hunting
                    // todo: could advance by j + 2 I think?
                    i = i + (uint)(j+2);
                }
                else
                {
                    var badBytes = new byte[i + 7 + payLoadLength];
                    Array.Copy(bytesToProcess, (int) (i-1), badBytes, 0, payLoadLength + 7);

                    if (PacketFailedCRC!=null)
                    {
                        PacketFailedCRC(this, new PacketCRCFailEventArgs(badBytes));
                    }

                    BadCrcPacketsReceived++;
                }
            }
        }


        private void OnPacketDecoded(byte[] packet, byte sequence)
        {
                if (PacketDecoded != null)
                {
                    PacketDecoded(this, new PacketDecodedEventArgs(packet, sequence));
                }

                PacketsReceived++;
        }
        
    }
}
