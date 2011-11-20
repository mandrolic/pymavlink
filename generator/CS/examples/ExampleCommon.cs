using System;
using System.IO;
using System.IO.Ports;

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