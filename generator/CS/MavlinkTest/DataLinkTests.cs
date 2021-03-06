﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MavLink;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MavlinkTest
{
    [TestClass]
    public class DataLinkTests
    {
        private Mavlink _dl;
        private List<MavlinkPacket> _decodedPackets;
        private TestStream _testStream;


        [TestInitialize]
        public void Setup()
        {
            _testStream = new TestStream();
            _dl = new Mavlink(_testStream);
            _decodedPackets = new List<MavlinkPacket>();
            _dl.PacketReceived += (sender, e) => _decodedPackets.Add(e);
            _dl.Start();
        }

        private void AddReadBytes(byte[] hb)
        {
            _testStream.RxQueue.Enqueue(hb);
            Thread.Sleep(100);
        }

        [TestMethod]
        public void NoBytesMeansNoPackets()
        {
            Assert.AreEqual(0,_decodedPackets.Count);
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

        private static byte[] VFRHudPacketData()
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
        public void VfrHudPacketDataPacketIsParsed()
        {
            AddReadBytes(VFRHudPacketData());
            Assert.AreEqual(1, _decodedPackets.Count);
        }

        [TestMethod]
        public void VfrHudInTwoParts()
        {
            var hb1 = VFRHudPacketData().Take(8).ToArray();
            var hb2 = VFRHudPacketData().Skip(8).ToArray();

            AddReadBytes(hb1);
            AddReadBytes(hb2);
            Assert.AreEqual(1, _decodedPackets.Count);
        }

        [TestMethod]
        public void HeartBeatPacketIsParsed()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
             AddReadBytes(hb);
             Assert.AreEqual(1, _decodedPackets.Count);
        }

        [TestMethod]
        public void PacketRxCountIncreases()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            Assert.AreEqual((UInt16)0, _dl.PacketsReceived);
            AddReadBytes(hb);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
        }

     

        [TestMethod]
        public void FragmentedPacketIsOK()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            var first = hb.Take(3).ToArray();
            var second = hb.Skip(3).ToArray();

            Assert.AreEqual((UInt16)0, _dl.PacketsReceived);
            AddReadBytes(first);
            Assert.AreEqual((UInt16)0, _dl.PacketsReceived);
            AddReadBytes(second);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
        }

        [TestMethod]
        public void LeadingDataIsOk()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            var first = Enumerable.Repeat((byte)42, 10);

            var ar = first.Concat(hb).ToArray();

            AddReadBytes(ar);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
        }

        [TestMethod]
        public void BadCrcBytesStopPacketReceived()
        {
            var hb = GoodMavlinkHeartbeatPacketData();
            hb[9] = 0; // screw the CRC byte
            AddReadBytes(hb);
            Assert.AreEqual((UInt16)0, _decodedPackets.Count);
            Assert.AreEqual((UInt16)0, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)1, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceiveAGoodPacketAfterABadCrc()
        {
            var badCrcPacket = GoodMavlinkHeartbeatPacketData();
            badCrcPacket[9] = 0; // screw the CRC byte
            AddReadBytes(badCrcPacket);

            AddReadBytes(GoodMavlinkHeartbeatPacketData());

            Assert.AreEqual((UInt16)1, _decodedPackets.Count);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)1, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceiveMultiplePacketsInOneHit()
        {
            var multipacket = GoodMavlinkHeartbeatPacketData().Concat(VFRHudPacketData()).ToArray();
            AddReadBytes(multipacket);

            Assert.AreEqual((UInt16)2, _decodedPackets.Count);
            Assert.AreEqual((UInt16)2, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceivePacketOneByteAtATime()
        {
            var xs = GoodMavlinkHeartbeatPacketData().Select(b => new byte[] { b });

            foreach (var oneByteArray in xs)
            {
                _testStream.RxQueue.Enqueue(oneByteArray);
                Thread.Sleep(10);
                
            }
            Assert.AreEqual(1, _decodedPackets.Count);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceiveMultiplePacketsOneByteAtATime()
        {
            var multipacket = GoodMavlinkHeartbeatPacketData()
                .Concat(VFRHudPacketData())
                .ToArray()
                .Select(b => new[] { b });

            foreach (var oneByteArray in multipacket)
            {
                _testStream.RxQueue.Enqueue(oneByteArray);
                Thread.Sleep(10);
                
            }
            Thread.Sleep(200);

            Assert.AreEqual(2, _decodedPackets.Count);
            Assert.AreEqual((UInt16)2, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);
        }

        [TestMethod]
        public void CanReceiveMultiplePacketsPartialSecondPacket()
        {
            var firstAndPartSecond = GoodMavlinkHeartbeatPacketData().Concat(VFRHudPacketData().Take(21)).ToArray();
            AddReadBytes(firstAndPartSecond);

            Assert.AreEqual((UInt16)1, _decodedPackets.Count);
            Assert.AreEqual((UInt16)1, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);

            byte[] newlyReceived = VFRHudPacketData().Skip(21).ToArray();
            AddReadBytes(newlyReceived);
            _testStream.RxQueue.Enqueue(newlyReceived);
            Thread.Sleep(100);


            Assert.AreEqual((UInt16)2, _decodedPackets.Count);
            Assert.AreEqual((UInt16)2, _dl.PacketsReceived);
            Assert.AreEqual((UInt16)0, _dl.BadCrcPacketsReceived);
            
        }

//        [TestMethod]
//        public void HeartBeatPacketIsPassedUpCorrectlyLenghthwise()
//        {
//            _testStream.RxQueue.Enqueue(GoodMavlinkHeartbeatPacketData());
//            Thread.Sleep(100);
//
//            var packet= _decodedPackets[0];
//            Assert.AreEqual(6,packet.Message );
//        }


//          [TestMethod]
//        public void HeartBeatPacketIsPassedUpContentwise()
//        {
//            AddReadBytes(GoodMavlinkHeartbeatPacketData());
//            _testStream.RxQueue.Enqueue(GoodMavlinkHeartbeatPacketData());
//
//            var packet= _decodedPackets[0];
//            Assert.AreEqual(0x07, packet[0]);
//            Assert.AreEqual(0x01, packet[1]);
//            Assert.AreEqual(0x00, packet[2]);
//            Assert.AreEqual(0x00, packet[3]);
//            Assert.AreEqual(0x03, packet[4]);
//            Assert.AreEqual(0x02, packet[5]);
//        }
        
        // Todo: test bad and good in same packet
    }
}
