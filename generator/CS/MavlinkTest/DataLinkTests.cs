using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MavlinkStructs;

namespace MavlinkTest
{
    [TestClass]
    public class DataLinkTests
    {
        private Mavlink_Link _dl;
        private List<byte[]> _decodedPackets;

        private void Setup()
        {
            _dl = new Mavlink_Link();
            _decodedPackets = new List<byte[]>();
            _dl.PacketDecoded += new PacketDecodedEventHandler(_dl_PacketDecoded);
        }

        void _dl_PacketDecoded(object sender, PacketDecodedEventArgs e)
        {
            _decodedPackets.Add(e.Payload);
        }


        [TestMethod]
        public void NoBytesMeansNoPackets()
        {
            Setup();
            _dl.AddReadBytes(new byte[] { });
            Assert.AreEqual(0,_decodedPackets.Count);
        }

        private byte[] GoodMavlinkHeartbeatPacketData()
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

        [TestMethod]
        public void VFRHudPacketDataPacketIsParsed()
        {
            Setup();
            var hb = VFRHudPacketData();
            _dl.AddReadBytes(hb);
            Assert.AreEqual(1, _decodedPackets.Count);
        }

        [TestMethod]
        public void VFRHudInTwoParts()
        {
            Setup();
            var hb1 = VFRHudPacketData().Take(8).ToArray();
            var hb2 = VFRHudPacketData().Skip(8).ToArray();

            _dl.AddReadBytes(hb1);
            _dl.AddReadBytes(hb2);
            Assert.AreEqual(1, _decodedPackets.Count);
        }

        [TestMethod]
        public void HeartBeatPacketIsParsed()
        {
            Setup();
            var hb = GoodMavlinkHeartbeatPacketData();
             _dl.AddReadBytes(hb);
             Assert.AreEqual(1, _decodedPackets.Count);
        }

        [TestMethod]
        public void PacketRxCountIncreases()
        {
            Setup();
            var hb = GoodMavlinkHeartbeatPacketData();
            Assert.AreEqual((UInt16)0, _dl.PacketsReceived);
            _dl.AddReadBytes(hb);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
        }

     

        [TestMethod]
        public void FragmentedPacketIsOK()
        {
            Setup();
            var hb = GoodMavlinkHeartbeatPacketData();
            var first = hb.Take(3).ToArray();
            var second = hb.Skip(3).ToArray();

            Assert.AreEqual((UInt16)0, _dl.PacketsReceived);
            _dl.AddReadBytes(first);
            Assert.AreEqual((UInt16)0, _dl.PacketsReceived);
            _dl.AddReadBytes(second);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
        }

        [TestMethod]
        public void LeadingDataIsOK()
        {
            Setup();
            var hb = GoodMavlinkHeartbeatPacketData();
            var first = Enumerable.Repeat((byte)42, 10);

            var ar = first.Concat(hb).ToArray();

            _dl.AddReadBytes(ar);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
        }

        [TestMethod]
        public void BadCrcBytesStopPacketReceived()
        {
            Setup();
            var hb = GoodMavlinkHeartbeatPacketData();
            hb[9] = 0; // screw the CRC byte
            _dl.AddReadBytes(hb);
            Assert.AreEqual((UInt16)0, _decodedPackets.Count);
            Assert.AreEqual((UInt16)0, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)1, _dl.BadCrcPacketsReceived);
        }


        [TestMethod]
        public void CanReceiveAGoodPacketAfterABadCrc()
        {
            Setup();
            
            var badCrcPacket = GoodMavlinkHeartbeatPacketData();
            badCrcPacket[9] = 0; // screw the CRC byte
            _dl.AddReadBytes(badCrcPacket);

            _dl.AddReadBytes(GoodMavlinkHeartbeatPacketData());

            Assert.AreEqual((UInt16)1, _decodedPackets.Count);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)1, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceiveMultiplePacketsInOneHit()
        {
            Setup();

            var multipacket = GoodMavlinkHeartbeatPacketData().Concat(VFRHudPacketData()).ToArray();
            _dl.AddReadBytes(multipacket);

            Assert.AreEqual((UInt16)2, _decodedPackets.Count);
            Assert.AreEqual((UInt16)2, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceivePacketOneByteAtATime()
        {
            Setup();
            var xs = GoodMavlinkHeartbeatPacketData().Select(b => new byte[] { b });

            foreach (var oneByteArray in xs )
                _dl.AddReadBytes(oneByteArray);

            Assert.AreEqual((UInt16)1, _decodedPackets.Count);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceiveMultiplePacketsOneByteAtATime()
        {
            Setup();

            var multipacket = GoodMavlinkHeartbeatPacketData().Concat(VFRHudPacketData()).ToArray().Select(b => new byte[] { b });


            foreach (var oneByteArray in multipacket)
                _dl.AddReadBytes(oneByteArray);

            Assert.AreEqual((UInt16)2, _decodedPackets.Count);
            Assert.AreEqual((UInt16)2, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceiveMultiplePacketsPartialSecondPacket()
        {
            Setup();

            var firstAndPartSecond = GoodMavlinkHeartbeatPacketData().Concat(VFRHudPacketData().Take(21)).ToArray();
            _dl.AddReadBytes(firstAndPartSecond);

            Assert.AreEqual((UInt16)1, _decodedPackets.Count);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);

            _dl.AddReadBytes(VFRHudPacketData().Skip(21).ToArray());

            Assert.AreEqual((UInt16)2, _decodedPackets.Count);
            Assert.AreEqual((UInt16)2, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);
            
        }

        [TestMethod]
        public void HeartBeatPacketIsPassedUpCorrectlyLenghthwise()
        {
            Setup();
            _dl.AddReadBytes(GoodMavlinkHeartbeatPacketData());
            var packet= _decodedPackets[0];
            Assert.AreEqual(6,packet.Length );
        }


          [TestMethod]
        public void HeartBeatPacketIsPassedUpContentwise()
        {
            Setup();
            _dl.AddReadBytes(GoodMavlinkHeartbeatPacketData());
            var packet= _decodedPackets[0];
            Assert.AreEqual(0x07, packet[0]);
            Assert.AreEqual(0x01, packet[1]);
            Assert.AreEqual(0x00, packet[2]);
            Assert.AreEqual(0x00, packet[3]);
            Assert.AreEqual(0x03, packet[4]);
            Assert.AreEqual(0x02, packet[5]);
        }
        


        // Todo: test bad and good in same packet
    }
}
