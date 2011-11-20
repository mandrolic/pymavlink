using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MavLink;

namespace Mavlink_Monitor_Console
{
    /// <summary>
    /// Dumps all stuff from mavlink to the console
    /// </summary>
    public class ConsoleDumper
    {
        private readonly MavlinkNetwork _mavNet;

        private const string DefaultLineFormat = "Sys: {0} Comp: {1} Msg:{2}";

        public ConsoleDumper(MavlinkNetwork mavNet)
        {
            _mavNet = mavNet;
            _mavNet.PacketReceived += NetworkLayerPacketReceived;
        }

        static void NetworkLayerPacketReceived(object sender, MavlinkPacket e)
        {
            if (e.Message is MAVLink_vfr_hud_message)
                ShowMessage((MAVLink_vfr_hud_message)e.Message);
            else
                Console.WriteLine(DefaultLineFormat, e.SystemId, e.ComponentId, e.Message.GetType().Name);
        }

        private static void ShowMessage(MAVLink_vfr_hud_message msg)
        {
            const string lineFormat = "vfr_hud_message:  Alt: {0} Heading: {1} Throttle: {2}";
            Console.WriteLine(lineFormat,  msg.alt, msg.heading, msg.throttle);
        }
    }
}
