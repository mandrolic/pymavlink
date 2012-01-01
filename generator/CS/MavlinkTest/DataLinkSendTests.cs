using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MavLink;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MavlinkTest
{
    [TestClass]
    public class DataLinkSendTests
    {
        private Mavlink _dut;
        private List<MavlinkPacket> _decodedPackets;
        private TestStream _testStream;

        [TestInitialize]
        public void Setup()
        {
            _testStream = new TestStream();
            _dut = new Mavlink(_testStream);
            _dut.PacketReceived  += (sender, e) => _decodedPackets.Add(e);
            _decodedPackets = new List<MavlinkPacket>();
            _dut.Start();
        }


        [TestMethod]
        public void HeartBeatDataRoundTrip()
        {
            var hbBytes = GoodMavlinkHeartbeatPacketData();
            
            _dut.txPacketSequence = 0x98; // hack to sync up with the real packet sequence no
            
            //_dl.AddReadBytes(hbBytes);
            _testStream.RxQueue.Enqueue(hbBytes);
            Thread.Sleep(200);


            var netPacket =  _decodedPackets[0];
            //var sendBytes = _dl.SendPacket(netPacket);
            
            _dut.Send(netPacket);
            
            var sendBytes = _testStream.SentBytes.SelectMany(b => b).ToArray();
            CollectionAssert.AreEqual(hbBytes, sendBytes);
        }

        [TestMethod]
        public void VfrHudPacketDataRoundTrip()
        {
            var packetBytes = VFRHudPacketData();
            _dut.txPacketSequence = 0xeb; // hack to sync up with the real packet sequence no

            //_dl.AddReadBytes(hbBytes);
            _testStream.RxQueue.Enqueue(packetBytes);
            Thread.Sleep(100);
            
            var netPacket = _decodedPackets[0];
            //var sendBytes = _dl.SendPacket(netPacket);
            _dut.Send(netPacket);
            var sendBytes = _testStream.SentBytes.SelectMany(b => b).ToArray();
            CollectionAssert.AreEqual(packetBytes, sendBytes);
        }

        private static byte[] GoodMavlinkHeartbeatPacketData()
        {
            return new byte[] {
                0x55,   // start byte
                0x03,   // length (of the data, not this packet)
                0x98,   // packet sequence no - keep this 98 to preserve crc
                0x07,   // System ID
                0x01,   // Component ID
                0x00,   // Message ID = 0 = heartbeat
                0x00,   // Type = 0 = generic
                0x03,   // Autopilot = 3 = ardu
                0x02,   // Mav Type = from MAV_TYPE ENUM
                0x8b,   // CRC high
                0x05    // CRC low
            };
        }


        private byte[] VFRHudPacketData()
        {
            return new byte[] {
                0x55, 
                0x14, // length 
                0xeb, // packet sequence
                0x07, // System ID
                0x01, // Component ID
                0x4a, // Message ID = 74 = VFR Hud
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x29, 
                0x00, 0x00, 
                0x3e, 0xa8, 0xf5, 0xc3, 
                0x00, 0x00, 0x00, 0x00, 
                0xee, 0x6f,
            };
        }
    }
}