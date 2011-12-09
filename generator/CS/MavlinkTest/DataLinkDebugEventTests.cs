using System.Collections.Generic;
using MavLink;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MavlinkTest
{
    [TestClass]
    public class DataLinkDebugEventTests
    {
        private Mavlink_Link _dl;
        private List<byte[]> _decodedPackets;
        private TestStream _testStream;
        private List<PacketCRCFailEventArgs> unusedEvents;
        private List<PacketCRCFailEventArgs> failCrcEvents;

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

        [TestInitialize]
        public void SetUp()
        {
            failCrcEvents = new List<PacketCRCFailEventArgs>();
            unusedEvents=new List<PacketCRCFailEventArgs>();
            _dl = new Mavlink_Link(null);
            _dl.BytesUnused += _dl_BytesUnused;
            _dl.PacketFailedCRC += new PacketCRCFailEventHandler(_dl_PacketFailedCRC);
        }

        void _dl_PacketFailedCRC(object sender, PacketCRCFailEventArgs e)
        {
            failCrcEvents.Add(e);
        }

        [TestMethod]
        public void UnusedBytesBetweenPacketsWorks()
        {
            _dl.AddReadBytes(GoodMavlinkHeartbeatPacketData());
            _dl.AddReadBytes(new byte[] {1,2,3,4,5});
            _dl.AddReadBytes(GoodMavlinkHeartbeatPacketData());

        }

        [TestMethod]
        public void BadCrcPakcetEvent()
        {
            var goo = GoodMavlinkHeartbeatPacketData();
            goo[8] = 42;
            _dl.AddReadBytes(goo);
            _dl.AddReadBytes(GoodMavlinkHeartbeatPacketData());

            Assert.AreEqual(1, failCrcEvents.Count);
            Assert.AreEqual(goo.Length,failCrcEvents[0].BadPacket.Length);

        }

        void _dl_BytesUnused(object sender, PacketCRCFailEventArgs e)
        {
            unusedEvents.Add(e);
        }
    }
}