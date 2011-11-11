﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace MavlinkStructs
{
   public class PacketDecodedEventArgs
    {
       public PacketDecodedEventArgs(byte[] payload)
        {
            Payload = payload;
        }

        public byte [] Payload;
    }

   public delegate void PacketDecodedEventHandler(object sender, PacketDecodedEventArgs e);


/// <summary>
/// This can send and receive bytes over a mavlink connection. Escaping, CRC etc are all done here
/// </summary>
   public interface IDataLink
   {
       event PacketDecodedEventHandler PacketDecoded;
       void SendPacket(byte[] packetData);
   }

   public class Mavlink_Link : IDataLink
    {
       private Stream _ioStream;

        private byte[] leftovers;

        public UInt32 PacketsReceived { get; private set; }
        public UInt32 BadCrcPacketsReceived { get; private set; }

        public event PacketDecodedEventHandler PacketDecoded;


        public byte packetSequence; // public so it can be manipulated for testing
       private Thread _receiveThread;

     

       public Mavlink_Link(Stream stream)
       {
           _ioStream = stream;

            _receiveThread = new Thread(ReceiveBytes);
           _receiveThread.IsBackground = true;
           _receiveThread.Start();

            leftovers = new byte[] { };
        }

        public void ReceiveBytes()
        {
            // todo; graceful thread cancellation etc

            byte[] inbuf;
            while (true)
            {
                inbuf = new byte[4];
                _ioStream.Read(inbuf, 0, inbuf.Length);
                AddReadBytes(inbuf);
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
            outBytes[2] = packetSequence;

            packetSequence = unchecked(packetSequence++);

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
        private void AddReadBytes(byte[] newlyReceived)
        {
            uint i = 0;

            // copy the old and new into a contiguous array
            // This is pretty inefficient...
            var bytesToProcess = new byte[newlyReceived.Length + leftovers.Length];
            int j = 0;

            for (i = 0; i < leftovers.Length; i++)
                bytesToProcess[j++] = leftovers[i];

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
                    // No start byte found in all our bytes. Exit.
                    leftovers = new byte[] { };
                    return;
                }

                // We need at least the minimum length of a packet to process it. 
                // The minimum packet length is 8 bytes for acknowledgement packets without payload
                if (bytesToProcess.Length - i < 8)
                {
                    leftovers = new byte[bytesToProcess.Length - i];
                    j = 0;
                    while (i < bytesToProcess.Length)
                        leftovers[j++] = bytesToProcess[i++];
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

                // If we don't have enough bytes in this packet to satisfy the packet lenghth,
                // then dump the whole lot in the leftovers and do nothing else
                if (payLoadLength > (bytesToProcess.Length - i - 8)) // payload + 'overhead' bytes (crc, system etc)
                {
                    // back up to the start char for next cycle
                    j = 0;

                    leftovers = new byte[bytesToProcess.Length - i];

                    for (; i < bytesToProcess.Length; i++)
                    {
                        leftovers[j++] = bytesToProcess[i];
                    }
                    return;
                }

                i++;
                // The minimum packet length is 8 bytes for acknowledgement packets without payload

                // Check the CRC. Does not include the starting 'U' byte but does include the length
                var crc1 = Mavlink_Crc.Calculate(bytesToProcess, (UInt16)(i), (UInt16)(payLoadLength + 5));

                byte crc_high = (byte)(crc1 & 0xFF);
                byte crc_low = (byte)(crc1 >> 8);

                if (bytesToProcess[i + 5 + payLoadLength] == crc_high && bytesToProcess[i + 6 + payLoadLength] == crc_low)
                {
                    // This is used for data drop outs metrics, not packet windows
                    // so we should consider this here. No need to pass up to Network layer though
                    var packetSequence = bytesToProcess[++i];
                    i++;
                    var packet = new byte[payLoadLength + 3];  // +3 because we are going to send up the sys and comp id and msg type with the data

                    for (j = 0; j < packet.Length; j++)
                        packet[j] = bytesToProcess[i + j];

                    OnPacketDecoded(packet);
                   
                    // TODO: advance i here by j to avoid unecessary hunting!
                }
                else
                {
                    BadCrcPacketsReceived++;
                }
            }
        }


        private void OnPacketDecoded(byte[] packet)
        {
                if (PacketDecoded != null)
                {
                    PacketDecoded(this, new PacketDecodedEventArgs(packet));
                }

                PacketsReceived++;
        }
        
    }
}