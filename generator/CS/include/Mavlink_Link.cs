﻿using System;
using System.IO;
using System.Threading;
#if MF_FRAMEWORK_VERSION_V4_1
using Microsoft.SPOT;
#endif

namespace MavLink
{
   public class Mavlink
    {
       private readonly Stream _ioStream;
       //private volatile bool stopThread;
       private bool stopThread; // no volatile in uFramework
       private byte[] _leftovers;

       private readonly MavlinkFactory _encoder;

       public event PacketReceivedEventHandler PacketReceived;

       /// <summary>
       /// Total number of packets successfully received so far
       /// </summary>
       public UInt32 PacketsReceived { get; private set; }

       /// <summary>
       /// Total number of packets which have been rejected due to a failed crc
       /// </summary>
       public UInt32 BadCrcPacketsReceived { get; private set; }

      
       /// <summary>
       /// Raised when a packet does not pass CRC
       /// </summary>
        public event PacketCRCFailEventHandler PacketFailedCRC;

       /// <summary>
       /// Raised when a number of bytes are passed over and cannot be used to decode a packet
       /// </summary>
        public event PacketCRCFailEventHandler BytesUnused;


        public byte txPacketSequence; // public so it can be manipulated for testing

        private readonly Thread _receiveThread;

       /// <summary>
       /// Create a new Mavlink Link with the given (open) stream
       /// </summary>
       /// <param name="stream"></param>
       public Mavlink(Stream stream)
       {
           _ioStream = stream;
           _receiveThread = new Thread(ReceiveBytes);
           _leftovers = new byte[] {};
           _encoder = new MavlinkFactory(false);
       }




       /// <summary>
       /// Start the link receive thread. 
       /// </summary>
       public void Start()
       {
           _receiveThread.Start();
       }

       /// <summary>
       /// Request the link receive thread to stop
       /// </summary>
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

                ParseBytes(processBuf);
            }
        }

       // Send a raw message over the link - add start byte, lenghth, crc and other link layer stuff
        private void SendPacketLinkLayer(byte[] packetData)
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


        public void Send(MavlinkPacket mavlinkPacket)
        {
            var bytes = _encoder.Serialize(mavlinkPacket.Message, mavlinkPacket.SystemId, mavlinkPacket.ComponentId);
            SendPacketLinkLayer(bytes);
        }


        // Process a raw packet in it's entirety in the given byte array
        // if deserialization is successful, then the packetdecoded event will be raised
        private MavlinkPacket ProcessPacketBytes(byte[] packetBytes, byte rxPacketSequence)
        {
            //	 System ID	 1 - 255
            //	 Component ID	 0 - 255
            //	 Message ID	 0 - 255
            //   6 to (n+6)	 Data	 (0 - 255) bytes
            var packet = new MavlinkPacket
            {
                SystemId = packetBytes[0],
                ComponentId = packetBytes[1],
                SequenceNumber = rxPacketSequence,
                Message = _encoder.Deserialize(packetBytes, 2)
            };

            if (PacketReceived != null)
            {
                PacketReceived(this, packet);
            }

            return packet;
        }

        /// <summary>
        /// Process latest bytes from the stream. Received packets will be raised in the event
        /// </summary>
        public void ParseBytes(byte[] newlyReceived)
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
                int huntStartPos = (int) i;

                while (i < bytesToProcess.Length && bytesToProcess[i] != 0x55)
                    i++;

                if (i == bytesToProcess.Length)
                {
                    // No start byte found in all our bytes. Dump them, Exit.
                    _leftovers = new byte[] { };
                    return;
                }

                if (i > huntStartPos)
                {
                    // if we get here then are some bytes which this code thinks are 
                    // not interesting and would be dumped. For diagnostics purposes,
                    // lets pop these bytes up in an event.
                    if (BytesUnused != null)
                    {
                        var badBytes = new byte[i - huntStartPos];
                        Array.Copy(bytesToProcess, huntStartPos, badBytes, 0, (int)(i - huntStartPos));
                        BytesUnused(this, new PacketCRCFailEventArgs(badBytes, bytesToProcess.Length - huntStartPos));
                    }
                }

                // We need at least the minimum length of a packet to process it. 
                // The minimum packet length is 8 bytes for acknowledgement packets without payload
                // if we don't have the minimum now, go round again
                if (bytesToProcess.Length - i < 8)
                {
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

                byte crcHigh = (byte)(crc1 & 0xFF);
                byte crcLow = (byte)(crc1 >> 8);

                if (bytesToProcess[i + 5 + payLoadLength] == crcHigh && bytesToProcess[i + 6 + payLoadLength] == crcLow)
                {
                    // This is used for data drop outs metrics, not packet windows
                    // so we should consider this here. 
                    // We pass up to subscribers only as an advisory thing
                    var rxPacketSequence = bytesToProcess[++i];
                    i++;
                    var packet = new byte[payLoadLength + 3];  // +3 because we are going to send up the sys and comp id and msg type with the data

                    for (j = 0; j < packet.Length; j++)
                        packet[j] = bytesToProcess[i + j];

                    var debugArray = new byte[payLoadLength + 7];
                    Array.Copy(bytesToProcess, (int) (i - 3), debugArray, 0, debugArray.Length);

                    //OnPacketDecoded(packet, rxPacketSequence, debugArray);

                    ProcessPacketBytes(packet, rxPacketSequence);
                    
                    PacketsReceived++;

                    // clear leftovers, just incase this is the last packet
                    _leftovers=new byte[] {};

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
                        PacketFailedCRC(this, new PacketCRCFailEventArgs(badBytes, (int) (bytesToProcess.Length-i-1)));
                    }

                    BadCrcPacketsReceived++;
                }
            }
        }  
    }


   ///<summary>
   /// Describes an occurance when a packet fails CRC
   ///</summary>
   public class PacketCRCFailEventArgs : EventArgs
   {
       ///<summary>
       ///</summary>
       public PacketCRCFailEventArgs(byte[] badPacket, int offset)
       {
           BadPacket = badPacket;
           Offset = offset;
       }

       /// <summary>
       /// The bytes that filed the CRC, including the starting character
       /// </summary>
       public byte[] BadPacket;

       /// <summary>
       /// The offset in bytes where the start of the block begins, e.g 
       /// 50 would mean the block of badbytes would start 50 bytes ago 
       /// in the stread. No negative sign is necessary
       /// </summary>
       public int Offset;
   }

   ///<summary>
   /// Handler for an PacketFailedCRC Event
   ///</summary>
   public delegate void PacketCRCFailEventHandler(object sender, PacketCRCFailEventArgs e);


   public delegate void PacketReceivedEventHandler(object sender, MavlinkPacket e);


    ///<summary>
    /// Represents a Mavlink message - both the message object itself
    /// and the identified sending party
    ///</summary>
    public class MavlinkPacket
    {
        /// <summary>
        /// The sender's system ID
        /// </summary>
        public int SystemId;

        /// <summary>
        /// The sender's component ID
        /// </summary>
        public int ComponentId;

        /// <summary>
        /// The sequence number received for this packet
        /// </summary>
        public byte SequenceNumber;


        /// <summary>
        /// Time of receipt
        /// </summary>
        public DateTime TimeStamp;

        /// <summary>
        /// Object which is the mavlink message
        /// </summary>
        public object Message;
    }
}
