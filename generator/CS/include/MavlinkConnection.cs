namespace Mavlink
{
    /// <summary>
    /// A connection to a specific Mavlink system
    /// </summary>
    public class MavlinkConnection
    {
        private readonly MavlinkNetwork _mavlinkNetwork;
        private readonly int _srcSystemId;
        private readonly int _srcComponentId;
        private readonly int _targetSystemId;

        public delegate void MavConnectionReceivedEventHandler(object sender, object e);

        public event MavConnectionReceivedEventHandler PacketReceived;

        /// <summary>
        /// Create mavlink connection with explicit system and component ids
        /// </summary>
        public MavlinkConnection(MavlinkNetwork mavlinkNetwork, int srcSystemId, int srcComponentId, int tgtSystemId)
        {
            _mavlinkNetwork = mavlinkNetwork;
            _srcSystemId = srcSystemId;
            _srcComponentId = srcComponentId;
            _targetSystemId = tgtSystemId;

            mavlinkNetwork.PacketReceived += mavlinkNetwork_PacketReceived;
        }

        void mavlinkNetwork_PacketReceived(object sender, MavlinkPacket e)
        {
            if (e.SystemId==_srcSystemId && e.ComponentId==_srcComponentId && PacketReceived!=null)
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