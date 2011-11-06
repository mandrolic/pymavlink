using System;
using System.Text;
using MavLink;

namespace MavlinkStructs
{
    public delegate void PacketReceivedEventHandler(object sender, MavlinkPacket e);


    public class MavlinkPacket
    {
        public int SystemId;
        public int ComponentId;
        public object Message;
    }

    public class Mavlink_Network
    {
        IDataLink _linkLayer;
        private IMavlinkEncoder _encoder;

        public event PacketReceivedEventHandler PacketReceived;

        public Mavlink_Network(IDataLink linkLayer,IMavlinkEncoder encoder)
        {
            _linkLayer = linkLayer;
            _encoder = encoder;
            _linkLayer.PacketDecoded += new PacketDecodedEventHandler(_linkLayer_PacketDecoded);
        }

        public byte[] Send(MavlinkPacket mavlinkPacket)
        {
            return _encoder.Serialize(mavlinkPacket.Message, mavlinkPacket.SystemId, mavlinkPacket.ComponentId);
        }

        void _linkLayer_PacketDecoded(object sender, PacketDecodedEventArgs e)
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
            {
                //Console.WriteLine("No Serialiser found");
                return null;
            }
            else
            {
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
    }

}
