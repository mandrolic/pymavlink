using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using Mavlink;
using MavLink;

namespace DumpParameters
{
    /*
     * Simple program to listen for Mavlink systems, and when
     * they are found, dump the parameter list etc
     * 
     * If the stream is a file, not a serial port, then a request
     * will not be made, but any param packets encountered will be
     * dumped
     */
    class Program
    {
        private static byte discoveredSystemId;
        private static byte discoveredCompId;
        private static ManualResetEvent hbReceived;

        static void Main(string[] args)
        {
            var mavStream = ExampleCommon.GetMavStreamFromArgs(args);
            var link = new Mavlink_Link(mavStream);
            var net = new MavlinkNetwork(link);
            var connection = new MavlinkConnection(net, 255, 1);

            hbReceived = new ManualResetEvent(false);
            connection.RemoteSystemDetected += SystemDetected;
            link.Start();
            
            Console.WriteLine("Waiting For hearbeat (10 second timeout)...");
            
            if (!hbReceived.WaitOne(TimeSpan.FromSeconds(10)))
            {
                Console.WriteLine("No heartbeats found");
                link.Stop();
                Environment.Exit(1);
            }

            Console.WriteLine("Heartbeat found. System ID: " + discoveredSystemId);

            if (mavStream.CanWrite)
            {
                Console.WriteLine("Sending param list request: " + discoveredSystemId);

                var req = new MAVLink_param_request_list_message()
                              {
                                  target_system = discoveredSystemId,
                                  target_component = discoveredCompId,
                              };

                connection.Send(req);
            }
            else
            {
                Console.WriteLine("Cannot send paramlist request. Scanning for params... ");
            }

            net.PacketReceived += DumpParamPacket;

            
            Console.ReadKey();
            link.Stop();
            mavStream.Close();
        }

        private static void DumpParamPacket(object sender, MavlinkPacket e)
        {
            if (e.Message is MAVLink_param_value_message)
            {
                var p = (MAVLink_param_value_message)e.Message;

                Console.WriteLine("\n\nReceived:");
                Console.WriteLine("ID: " + ByteArrayUtil.ToString(p.param_id));
                Console.WriteLine("Value: " + p.param_value);
                Console.WriteLine("(" + p.param_index + " of " + p.param_count + ")");
            }
          
        }


        static void SystemDetected(object sender, MAVLink_heartbeat_message hb)
        {
                discoveredSystemId =  (byte) ((MavlinkConnection) sender).TargetSystemId;
                hbReceived.Set();
        }
    }







    public static class ExampleCommon
    {
        public static Stream GetMavStreamFromArgs(string[] args)
        {
            Stream strm = null;

            if (args.Length == 0)
            {
                strm = Console.OpenStandardInput();
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("Usage (todo)");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (args[0] == "-S")
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("Usage (todo)");
                    Environment.Exit(1);
                }
                var comport = args[1];
                var baud = Convert.ToInt32(args[2]);
                var port = new SerialPort(comport, baud);
                port.Open();
                strm = port.BaseStream;
            }
            else
            {
                strm = File.OpenRead(args[1]);
            }
            return strm;
        }
    }
}
