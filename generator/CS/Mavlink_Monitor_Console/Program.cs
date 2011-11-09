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
            

            var link = new Mavlink_Link();
            var net = new Mavlink_Network(link, new MavlinkFactory());

            if (args.Length == 0)
            {
                Console.WriteLine("Usage bla bla");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var port = new SerialPort("COM7", 57600);
            port.Open();
            var stream = port.BaseStream;

            //var fileName = args[0];
            //var bytes = File.ReadAllBytes(fileName);

            var consoledumper = new ConsoleDumper(net);

            link.AddReadBytes(bytes);

            Console.ReadKey();
        }
    }
}
