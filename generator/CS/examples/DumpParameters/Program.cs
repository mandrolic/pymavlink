using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using MavlinkStructs;

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
        private static int discoveredSystemId;
        private static ManualResetEvent hbReceived;

        static void Main(string[] args)
        {
            var mavStream = ExampleCommon.GetMavStreamFromArgs(args);
            var link = new Mavlink_Link(mavStream);
            var net = new Mavlink_Network(link);

            hbReceived = new ManualResetEvent(false);
            discoveredSystemId = -1;
            net.PacketReceived += NetPacketReceived;

            Console.WriteLine("Waiting For hearbeat (10 second timeout)...");
            if (!hbReceived.WaitOne(TimeSpan.FromSeconds(10)))
            {
                Console.WriteLine("No heartbeats found");
                Environment.Exit(1);
            }

            Console.WriteLine("Heartbeat found. System ID: " + discoveredSystemId);

            if (mavStream.CanWrite)
            {
                var req = new MavLink.MAVLink_param_request_list_message();


            }

        }

        static void NetPacketReceived(object sender, MavlinkPacket e)
        {
            if (e.Message is MavLink.MAVLink_heartbeat_message)
            {
                discoveredSystemId = e.SystemId;
                hbReceived.Set();
            }
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
