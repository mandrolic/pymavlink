using System;

namespace MavLink
{
    public class MavlinkFactory
    {
        public MavlinkFactory(bool isLittleEndian)
        {
            MavLink_Deserializer.SetDataIsLittleEndian(isLittleEndian);
            MavLink_Serializer.SetDataIsLittleEndian(isLittleEndian);
        }

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
            var packetGen = MavLink_Serializer.SerializerLookup[message.GetType()];

            if (packetGen == null)
                throw new Exception("No serializer found for type " + message.GetType());

            var buff = new byte[256];

            buff[0] = (byte)systemId;
            buff[1] = (byte)componentId;

            var endPos = 3;
            var msgId = (packetGen as MavlinkPacketSerializeFunc).Invoke(buff, ref endPos, message);

            buff[2] = (byte)msgId;  

            var resultBytes = new byte[endPos];
            Array.Copy(buff, resultBytes, endPos);

            return resultBytes;
        }
    }
}
