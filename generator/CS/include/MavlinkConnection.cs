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
        private readonly MavlinkNetwork _mavlinkNetwork;
        private readonly int _srcSystemId;
        private readonly int _srcComponentId;

        public delegate void MavConnectionReceivedEventHandler(object sender, object e);
        public delegate void MavConnectionRemoteSystemDetectedEventHandler(object sender, Msg_heartbeat hb);

        public event MavConnectionReceivedEventHandler PacketReceived;
        public event MavConnectionRemoteSystemDetectedEventHandler RemoteSystemDetected;

        /// <summary>
        /// Create mavlink connection with explicit system and component ids
        /// </summary>
        public MavlinkConnection(MavlinkNetwork mavlinkNetwork, int srcSystemId, int srcComponentId)
        {
            _mavlinkNetwork = mavlinkNetwork;
            _srcSystemId = srcSystemId;
            _srcComponentId = srcComponentId;

            mavlinkNetwork.PacketReceived += mavlinkNetwork_PacketReceived;
        }

      

        void mavlinkNetwork_PacketReceived(object sender, MavlinkPacket e)
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

        public void Send(object message)
        {
            var mvp = new MavlinkPacket
                          {
                              ComponentId = _srcComponentId,
                              SystemId = _srcSystemId,
                              Message = message
                          };

            _mavlinkNetwork.Send(mvp);
        }



    }


}