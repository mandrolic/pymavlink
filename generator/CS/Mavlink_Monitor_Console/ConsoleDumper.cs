using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MavlinkStructs;
using MavLink;

namespace Mavlink_Monitor_Console
{
    /// <summary>
    /// Dumps all stuff from mavlink to the console
    /// </summary>
    public class ConsoleDumper
    {
        private Mavlink_Network _mavNet;

        private static string lineFormat = "Sys: {0} Comp: {1} Msg:{2}";

        public ConsoleDumper(Mavlink_Network mavNet)
        {
            _mavNet = mavNet;
            _mavNet.PacketReceived += new PacketReceivedEventHandler(networkLayer_PacketReceived);

                 }

        void networkLayer_PacketReceived(object sender, MavlinkPacket e)
        {
            if (e.Message is MAVLink_vfr_hud_message)
                ShowMessage((MAVLink_vfr_hud_message)e.Message);
            else
                Console.WriteLine(lineFormat, e.SystemId, e.ComponentId, e.Message.GetType().Name);
        }

        private void ShowMessage(MAVLink_vfr_hud_message msg)
        {
            string lineFormat = "Alt: {0} Heading: {1} Throttle: {2}";
            Console.WriteLine(lineFormat,  msg.alt, msg.heading, msg.throttle);
        }
    }
}
