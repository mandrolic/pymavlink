using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MavLink;

namespace MavlinkTest
{
    [TestClass]
    public class DataLinkNetworkIntegrationTests
    {
        private MavlinkNetwork _nt;
        private Mavlink_Link _dl;
        private List<MavlinkPacket> _packetsRxed;
        private TestStream _testStream;

        [TestInitialize]
        public void Setup()
        {
            _packetsRxed = new List<MavlinkPacket>();
            _testStream = new TestStream();
            _dl = new Mavlink_Link(_testStream);
            _nt = new MavlinkNetwork(_dl, new MavlinkFactory());
            _nt.PacketReceived += (sender, e) => _packetsRxed.Add(e);
            _dl.Start();
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


        [TestMethod]
        public void DecodedGoodPacketRaisesReceivedEvent()
        {
            _testStream.RxQueue.Enqueue(GoodMavlinkHeartbeatPacketData());
            Thread.Sleep(100);
            Assert.AreEqual(1, _packetsRxed.Count);
        }


        [TestMethod]
        public void SystemIdIsCorrect()
        {
            _testStream.RxQueue.Enqueue(GoodMavlinkHeartbeatPacketData());
            Thread.Sleep(100);
            Assert.AreEqual(7, _packetsRxed[0].SystemId);
        }


        [TestMethod]
        public void ComponentIdIsCorrect()
        {
            _testStream.RxQueue.Enqueue(GoodMavlinkHeartbeatPacketData());
            Thread.Sleep(100);
            Assert.AreEqual(1, _packetsRxed[0].ComponentId);
        }

        [TestMethod]
        public void HeartBeatSerialisation()
        {
            _dl.packetSequence = 0x98; // hack to sync up with the real packet sequence no

            var packet = new MAVLink_heartbeat_message { autopilot=3, type=0, mavlink_version=2 };


             _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });

            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            var hbBytes = GoodMavlinkHeartbeatPacketData();

            CollectionAssert.AreEqual(hbBytes, dlbytes);
        }


        private byte[] VfrHudPacketData()
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
           
            _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            //_dl.AddReadBytes(dlbytes);
            _testStream.RxQueue.Enqueue(dlbytes);
            Thread.Sleep(100);


            Assert.AreEqual(1, _packetsRxed.Count);

            var mavPacket = _packetsRxed[0];
            Assert.AreEqual(7, mavPacket.SystemId);
            Assert.AreEqual(1, mavPacket.ComponentId);

            Assert.IsInstanceOfType(mavPacket.Message, typeof(MAVLink_vfr_hud_message));

            var msg = (MAVLink_vfr_hud_message)mavPacket.Message;

            Assert.AreEqual(42, msg.throttle);

            Assert.AreEqual(4.4f, msg.climb);
        }
       
        [TestMethod]
        public void RoundTripMavLinkScaledPressureMessage()
        {
            var packet = new MAVLink_scaled_pressure_message
            {
                usec = 123456UL,
                press_abs = 1234.56F,
                press_diff = -56.7F,
                temperature = -4
            };
           
            _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            //_dl.AddReadBytes(dlbytes);
            _testStream.RxQueue.Enqueue(dlbytes);
            Thread.Sleep(100);


            Assert.AreEqual(1, _packetsRxed.Count);

            var mavPacket = _packetsRxed[0];

            Assert.IsInstanceOfType(mavPacket.Message, typeof(MAVLink_scaled_pressure_message));

            var msg =(MAVLink_scaled_pressure_message) mavPacket.Message ;

            Assert.AreEqual(123456UL, msg.usec);
            Assert.AreEqual(1234.56F, msg.press_abs);
            Assert.AreEqual(-56.7F, msg.press_diff);
            Assert.AreEqual(-4, msg.temperature);
        }


         
        [TestMethod]
        public void RoundTripMavLinkStatustextMessage()
        {
            var packet = new MAVLink_statustext_message
            {
               severity = 1,
               text = ByteArrayUtil.FromString("hello")
            };
           
            _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();

            _testStream.RxQueue.Enqueue(dlbytes);
            Thread.Sleep(100);


            Assert.AreEqual(1, _packetsRxed.Count);

            var mavPacket = _packetsRxed[0];

            var msg = (MAVLink_statustext_message)mavPacket.Message;

            var str = ByteArrayUtil.ToString(msg.text);
            Assert.AreEqual("hello",str);

        }


        [TestMethod]
        public void RoundTripMavLinkParamValueMessage()
        {
            var packet = new MAVLink_param_value_message
            {
                param_count = 3,
                param_index = 1,
                param_value = 4.4F,
                param_id = ByteArrayUtil.FromString("Some Param")
            };

            _nt.Send(new MavlinkPacket { SystemId = 7, ComponentId = 1, Message = packet });
            var dlbytes = _testStream.SentBytes.SelectMany(b => b).ToArray();
            _testStream.RxQueue.Enqueue(dlbytes);
            Thread.Sleep(100);

            Assert.AreEqual(1, _packetsRxed.Count);

            var mavPacket = _packetsRxed[0];

            var msg = (MAVLink_param_value_message)mavPacket.Message;

            var str = ByteArrayUtil.ToString(msg.param_id);
            Assert.AreEqual("Some Param", str);
           

            Assert.AreEqual(3, msg.param_count);
            Assert.AreEqual(1, msg.param_index);
            Assert.AreEqual(4.4F, msg.param_value);
        }
    }
}
