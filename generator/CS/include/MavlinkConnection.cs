using System;
#if MF_FRAMEWORK
using Microsoft.SPOT;
#endif
using MavLink;


namespace MavLink
{
    /// <summary>
    /// A connection to a specific Mavlink system
    /// </summary>
    public class MavlinkConnection
    {
        private readonly Mavlink _mavlink;
        private readonly int _srcSystemId;
        private readonly int _srcComponentId;

        /// <summary>
        /// Handler for when packets are received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void MavConnectionReceivedEventHandler(object sender, object e);

        /// <summary>
        /// Handler for when heartbeats are deteceted from remote systems
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hb"></param>
        public delegate void MavConnectionRemoteSystemDetectedEventHandler(object sender, Msg_heartbeat hb);

        /// <summary>
        /// event for when packets are received
        /// </summary>
        public event MavConnectionReceivedEventHandler PacketReceived;

        /// <summary>
        /// event for when heartbeats are deteceted from remote systems
        /// </summary>
        public event MavConnectionRemoteSystemDetectedEventHandler RemoteSystemDetected;

        /// <summary>
        /// Create mavlink connection with explicit system and component ids
        /// </summary>
        public MavlinkConnection(Mavlink mavlink, int srcSystemId, int srcComponentId)
        {
            _mavlink = mavlink;
            _srcSystemId = srcSystemId;
            _srcComponentId = srcComponentId;

            mavlink.PacketReceived += mavlinkNetwork_PacketReceived;
        }

      

        private void mavlinkNetwork_PacketReceived(object sender, MavlinkPacket e)
        {
            if (e.Message is Msg_heartbeat)
            {
                if (RemoteSystemDetected != null)
                    RemoteSystemDetected(this, (Msg_heartbeat)e.Message);
            }

            
            //if (e.SystemId==_srcSystemId && e.ComponentId==_srcComponentId && PacketReceived!=null)
            if (PacketReceived!=null)
            {
                PacketReceived(this,e.Message);
            }
        }

        /// <summary>
        /// Send a mavlink message through this connection
        /// </summary>
        /// <param name="message">the message to send</param>
        public void Send(object message)
        {
            var mvp = new MavlinkPacket
                          {
                              ComponentId = _srcComponentId,
                              SystemId = _srcSystemId,
                              Message = message
                          };

            _mavlink.Send(mvp);
        }



    }


}