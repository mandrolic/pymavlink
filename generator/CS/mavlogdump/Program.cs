using System;
using System.IO.Ports;
using MavlinkStructs;
using System.IO;

namespace Mavlink_Monitor_Console
{
    /// <summary>
    /// Program to print out to console items received on a mavlink link
    /// or from a log file
    /// </summary>
    class Program
    {
        private const string FileUsage = "mavlogdump.exe -F [File]";
        private const string SerialUsage = "mavlogdump.exe -S [com port] [baudrate]";

        static void Main(string[] args)
        {
            Stream strm = null;
            
            if (args.Length == 0)
            {
                strm = Console.OpenStandardInput();
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("Usage '" + FileUsage + "' OR '" + SerialUsage + "' Use no arguments for stdin");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (args[0] == "-S" )
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("Error - argument syntax should be: '" + SerialUsage + "'");
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

            var link = new Mavlink_Link(strm);
            var net = new Mavlink_Network(link);

            var consoledumper = new ConsoleDumper(net);

            Console.ReadKey();
        }
    }
}
