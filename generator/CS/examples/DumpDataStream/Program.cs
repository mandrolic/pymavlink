using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using MavLink;

namespace DumpDataStream
{
    /*
     * Simple program to listen for Mavlink systems, and when
     * they are found, request and dump the parameter list 
     * 
     * If the stream is a file, not a serial port, then a request
     * will not be made, but any param packets encountered will be
     * dumped
     * 
     * -F "../../2011-09-22 07-00-05.tlog"
     * -S COM14 57600
     */
    class Program
    {
        private static Stream _mavStream;
        private static MavlinkNetwork _net;
        private static Mavlink_Link _link;

        static void Main(string[] args)
        {
            _mavStream = ExampleCommon.GetMavStreamFromArgs(args);


            _link = new Mavlink_Link(_mavStream);
            _net = new MavlinkNetwork(_link);

            var packetsReceived = Observable.FromEvent<PacketReceivedEventHandler, MavlinkPacket>(
                handler => (sender, e) => handler.Invoke(e),
                d => _net.PacketReceived += d,
                d => _net.PacketReceived -= d);

            Console.WriteLine("Waiting For hearbeat (10 second timeout)...");

            var heartbeats = packetsReceived.Where(p => p.Message is MAVLink_heartbeat_message);

            var paramsReceived = from hb in heartbeats
                                   .Take(1)
                                   .Do(SendRequestDataStreams)
                                   .Timeout(TimeSpan.FromSeconds(10))
                                 from message in packetsReceived
                                     .Where(m => m.SystemId == hb.SystemId)
                                     .Where(m => m.ComponentId == hb.ComponentId)
                                     .Select(p => p.Message)
                                 select message;

            paramsReceived
                .Do(m => Console.WriteLine("Msg rx: " +  m.GetType()))
                .DoWith<object, MAVLink_gps_status_message>(DumpGpsPacket)
                .DoWith<object, MAVLink_raw_imu_message>(DumpRawImuPacket)
                .DoWith<object, MAVLink_attitude_message>(DumpAttitudePacket)
                .Subscribe(
                        _ => { },
                        e => Console.WriteLine("Error: " + e.Message),
                        () => Console.WriteLine("Complete"));

            _link.Start();
            Console.ReadKey();
            _link.Stop();
            _mavStream.Close();
        }

        private static void DumpRawImuPacket(MAVLink_raw_imu_message m)
        {
            Console.WriteLine(string.Format("Raw IMU message: TimeStamp: {0}, xGyro:{1}, xAcc:{2}, xMag:{3}", 
                m.usec, m.xacc, m.xgyro, m.xmag ));

        }

        private static void DumpGpsPacket(MAVLink_gps_status_message msg)
        {
            Console.WriteLine(string.Format("GPS Status message: Visible: {0}", msg.satellites_visible));
        }

        private static void DumpAttitudePacket(MAVLink_attitude_message msg)
        {
            Console.WriteLine(string.Format("Attitude message: Pitch: {0}", msg.pitch));
        }
      

        private static void SendRequestDataStreams(MavlinkPacket hb)
        {
            //SendRequestDataStream(hb, MAV_DATA_STREAM.MAV_DATA_STREAM_ALL);
            //SendRequestDataStream(hb, MAV_DATA_STREAM.MAV_DATA_STREAM_EXTENDED_STATUS);
            //SendRequestDataStream(hb, MAV_DATA_STREAM.MAV_DATA_STREAM_POSITION);
            //SendRequestDataStream(hb, MAV_DATA_STREAM.MAV_DATA_STREAM_EXTRA1); // Attitude on APM
            SendRequestDataStream(hb, MAV_DATA_STREAM.MAV_DATA_STREAM_EXTRA2);   // VFR Hud on APM
            //SendRequestDataStream(hb, MAV_DATA_STREAM.MAV_DATA_STREAM_RAW_SENSORS);
            //SendRequestDataStream(hb, MAV_DATA_STREAM.MAV_DATA_STREAM_RC_CHANNELS);
        }

        private static void SendRequestDataStream(MavlinkPacket hb, MAV_DATA_STREAM id)
        {
            Console.WriteLine(string.Format("Requesting stream{0} from system: {1}, component: {2}",
                Enum.GetName(typeof(MAV_DATA_STREAM), id), hb.SystemId, hb.ComponentId));


            var req = new MAVLink_request_data_stream_message
                          {
                              target_system = (byte)hb.SystemId,
                              target_component = (byte)hb.ComponentId,
                              req_message_rate = 1,
                              start_stop = 1,
                              req_stream_id = (byte)id
                          };
            _net.Send(new MavlinkPacket { ComponentId = 1, SystemId = 7, Message = req });
        }
    }


    public static class RxExtenstions
    {
        public static IObservable<TSource> DoWith<TSource, TTarget>(this IObservable<TSource> source, Action<TTarget> action) where TTarget : TSource
        {
            return source.Do(o =>
                                 {
                                     if (o is TTarget)
                                         action((TTarget) o);
                                 });
        }

    }
}
