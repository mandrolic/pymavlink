using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
        static void Main(string[] args)
        {
            Stream strm = null;
            
            if (args.Length == 0)
            {
                strm = Console.OpenStandardInput();
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("Usage 'foo -F [File]' OR 'foo -S [com port] [baudrate]' Use no arguments for stdin");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (args[0] == "-S" )
            {
                var port = new SerialPort("COM7", 57600); // todo parse arge for things
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
