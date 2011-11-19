using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Mavlink;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MavLink;

namespace MavlinkTest
{
    [TestClass]
    public class NetworkTests
    {
        private MavlinkNetwork _nt;
        private Mock<IDataLink> dl;
        private List<MavlinkPacket> packetsRxed;

        [TestInitialize]
        public void Setup()
        {
            dl = new Mock<IDataLink>();
            packetsRxed = new List<MavlinkPacket>();

            _nt = new MavlinkNetwork(dl.Object,new MavlinkFactory());
            _nt.PacketReceived += (sender, e) => packetsRxed.Add(e);
        }

        private static byte[] GoodMavlinkHeartbeatPacketData()
        {
            return new byte[] {
                0x07,   // System ID
                0x01,   // Component ID
                0x00,   // Message ID = 0 = heartbeat
                0x00,   // Type = 0 = generic
                0x03,   // Autopilot = 3 = ardu
                0x02,   // Mav Type = from MAV_TYPE ENUM
            };
        }

        private static byte[] VFRHudPacketData()
        {
            return new byte[] {
                //55 14 eb
                0x07, // System ID
                0x01, // Component ID
                0x4a, // Message ID = 74 = VFR Hud
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 
                0x00, 0x29, 
                0x00, 0x00, 
                0x3e, 0xa8, 0xf5, 0xc3, 
                0x00, 0x00, 0x00, 0x00, 
                //0xee, 0x6f,
            };
        }

        [TestMethod]
        public void VFRHudPacketDataOK()
        {
            var hb = VFRHudPacketData();
            dl.Raise(d => d.PacketDecoded += null, this, new PacketDecodedEventArgs(hb));
            var fdrPcket = (MAVLink_vfr_hud_message)packetsRxed[0].Message;
            Assert.IsNotNull(fdrPcket);
            Assert.AreEqual(0F,fdrPcket.airspeed);
            Assert.AreEqual(0F,fdrPcket.groundspeed);
            Assert.AreEqual(41,fdrPcket.heading);
            Assert.AreEqual(0,fdrPcket.throttle);
            Assert.AreEqual(0.33,fdrPcket.alt,0.001);
            Assert.AreEqual(0F,fdrPcket.climb);
        }

        [TestMethod]
        public void DecodedGoodPacketRaisesReceivedEvent()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            dl.Raise(d => d.PacketDecoded += null,this, new PacketDecodedEventArgs(hb));
            Assert.AreEqual(1, packetsRxed.Count);
        }

        [TestMethod]
        public void HeartBeatMessageIsOk()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            dl.Raise(d => d.PacketDecoded += null, this, new PacketDecodedEventArgs(hb));
            var hbPcket = (MAVLink_heartbeat_message)packetsRxed[0].Message;
            Assert.IsNotNull(hbPcket);
            Assert.AreEqual(3, hbPcket.autopilot);
            Assert.AreEqual(0, hbPcket.type);
            Assert.AreEqual(2, hbPcket.mavlink_version);
            //Assert.IsTrue(MAV_AUTOPILOT.MAV_AUTOPILOT_GENERIC == hbPcket.autopilot);
        }

        [TestMethod]
        public void SystemIdIsCorrect()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            dl.Raise(d => d.PacketDecoded+=null, this,new PacketDecodedEventArgs(hb));
            Assert.AreEqual(7, packetsRxed[0].SystemId);
        }


        [TestMethod]
        public void ComponentIdIsCorrect()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            dl.Raise(d => d.PacketDecoded += null, this,new PacketDecodedEventArgs(hb));
            Assert.AreEqual(1, packetsRxed[0].ComponentId);
        }
    }
}
