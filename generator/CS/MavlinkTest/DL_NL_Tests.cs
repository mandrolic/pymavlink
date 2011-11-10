using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MavlinkStructs;
using MavLink;

namespace MavlinkTest
{
    [TestClass]
    public class DataLinkNetworkIntegrationTests
    {
        private Mavlink_Network _nt;
        private Mavlink_Link _dl;
        private List<MavlinkPacket> packetsRxed;
        private TestStream _testStream;

        private void Setup()
        {
            packetsRxed = new List<MavlinkPacket>();
            _testStream = new TestStream();
            _dl = new Mavlink_Link(_testStream);
            _nt = new Mavlink_Network(_dl, new MavlinkFactory());
            _nt.PacketReceived += _nt_PacketReceived;
        }

        void _nt_PacketReceived(object sender, MavlinkPacket e)
        {
            packetsRxed.Add(e);
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


        [TestMethod]
        public void DecodedGoodPacketRaisesReceivedEvent()
        {
            Setup();
            _testStream.RxQueue.Enqueue(GoodMavlinkHeartbeatPacketData());
            Thread.Sleep(100);
            Assert.AreEqual(1, packetsRxed.Count);
        }


        [TestMethod]
        public void SystemIdIsCorrect()
        {
            Setup();
            //_dl.AddReadBytes(GoodMavlinkHeartbeatPacketData());
            _testStream.RxQueue.Enqueue(GoodMavlinkHeartbeatPacketData());

            Assert.AreEqual(7, packetsRxed[0].SystemId);
        }


        [TestMethod]
        public void ComponentIdIsCorrect()
        {
            Setup();
            //_dl.AddReadBytes(GoodMavlinkHeartbeatPacketData());
            _testStream.RxQueue.Enqueue(GoodMavlinkHeartbeatPacketData());

            Assert.AreEqual(1, packetsRxed[0].ComponentId);
        }

        [TestMethod]
        public void HeartBeatSerialisation()
        {
            Setup();

            _dl.packetSequence = 0x98; // hack to sync up with the real packet sequence no

            var packet = new MavLink.MAVLink_heartbeat_message { autopilot=3, type=0, mavlink_version=2 };
            var ntbytes = _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            //var sendBytes = _dl.SendPacket(netPacket);
            _dl.SendPacket(ntbytes);
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            var hbBytes = this.GoodMavlinkHeartbeatPacketData();

            CollectionAssert.AreEqual(hbBytes, dlbytes);
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
        public void VfrPacketRoundTrip()
        {
            Setup();

            _dl.packetSequence = 0xeb; // hack to sync up with the real packet sequence no

            var packet = new MAVLink_vfr_hud_message
            {
                airspeed = 2.2f,
                groundspeed = 1.1f,
                heading = 0x29,
                throttle = 42,
                alt = 3.3f,
                climb = 4.4f
            };
           
            var ntbytes = _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            //var sendBytes = _dl.SendPacket(netPacket);
            _dl.SendPacket(ntbytes);
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            //_dl.AddReadBytes(dlbytes);
            _testStream.RxQueue.Enqueue(dlbytes);
            Thread.Sleep(100);


            Assert.AreEqual(1, packetsRxed.Count);

            var mavPacket = packetsRxed[0];
            Assert.AreEqual(7, mavPacket.SystemId);
            Assert.AreEqual(1, mavPacket.ComponentId);

            Assert.IsInstanceOfType(mavPacket.Message, typeof(MAVLink_vfr_hud_message));

            var msg = (MAVLink_vfr_hud_message)mavPacket.Message;

            Assert.AreEqual(42, msg.throttle);

            Assert.AreEqual(4.4f, msg.climb);
        }
       
        [TestMethod]
        public void RoundTrip_MAVLink_scaled_pressure_message()
        {
            Setup();

            var packet = new MAVLink_scaled_pressure_message
            {
                usec = 123456UL,
                press_abs = 1234.56F,
                press_diff = -56.7F,
                temperature = -4
            };
           
            var ntbytes = _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            //var sendBytes = _dl.SendPacket(netPacket);
            _dl.SendPacket(ntbytes);
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            //_dl.AddReadBytes(dlbytes);
            _testStream.RxQueue.Enqueue(dlbytes);
            Thread.Sleep(100);


            Assert.AreEqual(1, packetsRxed.Count);

            var mavPacket = packetsRxed[0];

            Assert.IsInstanceOfType(mavPacket.Message, typeof(MAVLink_scaled_pressure_message));

            var msg =(MAVLink_scaled_pressure_message) mavPacket.Message ;

            Assert.AreEqual(123456UL, msg.usec);
            Assert.AreEqual(1234.56F, msg.press_abs);
            Assert.AreEqual(-56.7F, msg.press_diff);
            Assert.AreEqual(-4, msg.temperature);
        }


         
        [TestMethod]
        public void RoundTrip_MAVLink_statustext_message()
        {
            Setup();

            var packet = new MAVLink_statustext_message
            {
               severity = 1,
               text = ByteArrayUtil.FromString("hello")
            };
           
            var ntbytes = _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            //var sendBytes = _dl.SendPacket(netPacket);
            _dl.SendPacket(ntbytes);
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            //_dl.AddReadBytes(dlbytes);
            _testStream.RxQueue.Enqueue(dlbytes);
            Thread.Sleep(100);


            Assert.AreEqual(1, packetsRxed.Count);

            var mavPacket = packetsRxed[0];

            var msg = (MAVLink_statustext_message)mavPacket.Message;

            var str = ByteArrayUtil.ToString(msg.text);
            Assert.AreEqual("hello",str);

        }


        [TestMethod]
        public void RoundTrip_MAVLink_param_value_message()
        {
            Setup();

            var packet = new MAVLink_param_value_message
            {
                param_count = 3,
                param_index = 1,
                param_value = 4.4F,
                param_id = ByteArrayUtil.FromString("Some Param")
            };

            var ntbytes = _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            //var sendBytes = _dl.SendPacket(netPacket);
            _dl.SendPacket(ntbytes);
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            //_dl.AddReadBytes(dlbytes);
            _testStream.RxQueue.Enqueue(dlbytes);


            Assert.AreEqual(1, packetsRxed.Count);

            var mavPacket = packetsRxed[0];

            var msg = (MAVLink_param_value_message)mavPacket.Message;

            var str = ByteArrayUtil.ToString(msg.param_id);
            Assert.AreEqual("Some Param", str);
           

            Assert.AreEqual(3, msg.param_count);
            Assert.AreEqual(1, msg.param_index);
            Assert.AreEqual(4.4F, msg.param_value);
        }
        
        
    }
}
