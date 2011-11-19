using System;
using MavLink;

namespace Mavlink
{
    public class MavlinkNetwork
    {
        private readonly IDataLink _linkLayer;
        private readonly IMavlinkEncoder _encoder;

        public event PacketReceivedEventHandler PacketReceived;

        // TODO: am I ever going to use a different mavlink factory. Don't think so must only be for testing
        public MavlinkNetwork(IDataLink linkLayer)
            : this(linkLayer, new MavlinkFactory())
        { }

        public MavlinkNetwork(IDataLink linkLayer,IMavlinkEncoder encoder)
        {
            _linkLayer = linkLayer;
            _encoder = encoder;
            _linkLayer.PacketDecoded += new PacketDecodedEventHandler(LinkLayerPacketDecoded);
        }

        public void Send(MavlinkPacket mavlinkPacket)
        {
            var bytes = _encoder.Serialize(mavlinkPacket.Message, mavlinkPacket.SystemId, mavlinkPacket.ComponentId);
            _linkLayer.SendPacket(bytes);
        }

        void LinkLayerPacketDecoded(object sender, PacketDecodedEventArgs e)
        {
            //	 System ID	 1 - 255
            //	 Component ID	 0 - 255
            //	 Message ID	 0 - 255
            //6 to (n+6)	 Data	 (0 - 255) bytes

            var packet = new MavlinkPacket();
            packet.SystemId = e.Payload[0];
            packet.ComponentId = e.Payload[1];

            packet.Message=_encoder.Deserialize(e.Payload, 2);

            if (PacketReceived != null)
            {
                PacketReceived(this, packet);
            }

        }
    }

    /// <summary>
    /// Something that can convert mavlink messages to and from byte arrays
    /// </summary>
    public interface IMavlinkEncoder
    {
        object Deserialize(byte[] bytes, int offset);
        byte [] Serialize(object mavLink, int systemId, int componentId);
    }


    public class MavlinkFactory : IMavlinkEncoder
    {
        public object Deserialize(byte[] bytes, int offset)
        {
            // first byte is the mavlink 
            var packetNum = (int)bytes[offset + 0];
            var obj = MavLink_Deserializer.DeserializerLookup[packetNum];

            if (obj == null)
            {
                //Debug.WriteLine("No Deserialiser for packet ID: " + packetNum);
                return null;
            }
            else
            {
                var packetGen = obj as MavlinkPacketDeserializeFunc;
                return packetGen.Invoke(bytes, offset + 1);
            }
        }

        public byte[] Serialize(object message, int systemId, int componentId)
        {
            var packetGen = (MavlinkPacketSerializeFunc)MavLink_Serializer.SerializerLookup[message.GetType()];

            if (packetGen == null)
                throw new ApplicationException("No serializer found for type " + message.GetType());

            var buff = new byte[256];

            buff[0] = (byte)systemId;
            buff[1] = (byte)componentId;

            var endPos = 3;
            var msgId = packetGen.Invoke(buff, ref endPos, message);

            buff[2] = (byte)msgId;  

            var resultBytes = new byte[endPos];
            Array.Copy(buff, resultBytes, endPos);

            return resultBytes;
        }
    }

    public delegate void PacketReceivedEventHandler(object sender, MavlinkPacket e);

    public class MavlinkPacket
    {
        public int SystemId;
        public int ComponentId;
        public object Message;
    }
}
