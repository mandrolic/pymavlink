﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MavLink;

namespace MavlinkTest
{
    [TestClass]
    public class NetworkTests
    {
        private Mavlink _mav;
        private List<MavlinkPacket> packetsRxed;

        [TestInitialize]
        public void Setup()
        {
            packetsRxed = new List<MavlinkPacket>();
            _mav = new Mavlink(null);
            _mav.PacketReceived += (sender, e) => packetsRxed.Add(e);
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

        [TestMethod]
        public void VFRHudPacketDataOK()
        {
            var hb = VFRHudPacketData();
            _mav.ParseBytes(hb);
            var fdrPcket = (Msg_vfr_hud)packetsRxed[0].Message;
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
            _mav.ParseBytes(hb);
            Assert.AreEqual(1, packetsRxed.Count);
        }

        [TestMethod]
        public void HeartBeatMessageIsOk()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            _mav.ParseBytes(hb);
            var hbPcket = (Msg_heartbeat)packetsRxed[0].Message;
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
            _mav.ParseBytes(hb);
            Assert.AreEqual(7, packetsRxed[0].SystemId);
        }


        [TestMethod]
        public void ComponentIdIsCorrect()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            _mav.ParseBytes(hb);
            Assert.AreEqual(1, packetsRxed[0].ComponentId);
        }
    }
}
