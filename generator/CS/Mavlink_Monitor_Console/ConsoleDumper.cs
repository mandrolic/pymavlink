using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MavlinkStructs;

namespace Mavlink_Monitor_Console
{
    /// <summary>
    /// Dumps all stuff from mavlink to the console
    /// </summary>
    public class ConsoleDumper
    {
        private Mavlink_Network _mavNet;

        private static string lineFormat = "Sys:{0} Comp:{1} Msg:{2}";

        public ConsoleDumper(Mavlink_Network mavNet)
        {
            _mavNet = mavNet;
            _mavNet.PacketReceived += new PacketReceivedEventHandler(networkLayer_PacketReceived);
        }

        void networkLayer_PacketReceived(object sender, MavlinkPacket e)
        {
            Console.WriteLine(lineFormat, e.SystemId, e.ComponentId, e.Message.GetType().Name);
        }
    }
}
