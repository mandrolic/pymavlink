﻿using System;
using System.IO;
using System.Reactive.Linq;
using MavLink;

namespace DumpParameters
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
            
            var heartbeats = packetsReceived.Where(p => p.Message is MAVLink_heartbeat_message);
            
            Console.WriteLine("Waiting For hearbeat (10 second timeout)...");

            var paramsReceived = from hb in heartbeats
                                     .Take(1)
                                     .Do(SendParamListRequest)
                                     .Timeout(TimeSpan.FromSeconds(10))
                      from message in packetsReceived
                          .Where(m => m.SystemId == hb.SystemId)
                          .Where(m => m.ComponentId == hb.ComponentId)
                          .Select(p => p.Message)
                          .OfType<MAVLink_param_value_message>()
                          .TakeWhile(p => p.param_index < p.param_count - 1 )
                      select message;

            paramsReceived.Subscribe(
                        DumpPacket, 
                        e => Console.WriteLine("Error: " + e.Message),
                        () => Console.WriteLine("Complete"));

            _link.Start();
            Console.ReadKey();
            _link.Stop();
            _mavStream.Close();
        }

        private static void DumpPacket(MAVLink_param_value_message p)
        {
            Console.WriteLine(string.Format("Param - ID: {0} Value: {1} ({2} of {3})", 
                ByteArrayUtil.ToString(p.param_id), p.param_value, p.param_index,p.param_count));
        }


        private static void SendParamListRequest(MavlinkPacket mavlinkPacket)
        {
            if (_mavStream.CanWrite)
            {
                Console.WriteLine("Sending param list request: " + mavlinkPacket.SystemId);

                var req = new MAVLink_param_request_list_message()
                {
                    target_system = (byte) mavlinkPacket.SystemId,
                    target_component = (byte) mavlinkPacket.ComponentId,
                };

                _net.Send(new MavlinkPacket { ComponentId = 1, SystemId = 255, Message = req });
            }
            else
            {
                Console.WriteLine("Cannot send paramlist request. Scanning for params... ");
            }
        }
    }
}
