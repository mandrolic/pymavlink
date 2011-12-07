﻿using System;

namespace MavLink
{
    ///<summary>
    /// Class that will code/decode mavlink packets 
    ///</summary>
    public class MavlinkNetwork
    {
        private readonly IDataLink _linkLayer;
        private readonly IMavlinkEncoder _encoder;

        public event PacketReceivedEventHandler PacketReceived;

        // TODO: am I ever going to use a different mavlink factory. Don't think so must only be for testing
        public MavlinkNetwork(IDataLink linkLayer)
            : this(linkLayer, new MavlinkFactory(false))
        { }

        public MavlinkNetwork(IDataLink linkLayer, IMavlinkEncoder encoder)
        {
            _linkLayer = linkLayer;
            _encoder = encoder;
            _linkLayer.PacketDecoded += (sender, e) => ProcessPacketBytes(e.Payload);

        }

        public void Send(MavlinkPacket mavlinkPacket)
        {
            var bytes = _encoder.Serialize(mavlinkPacket.Message, mavlinkPacket.SystemId, mavlinkPacket.ComponentId);
            _linkLayer.SendPacket(bytes);
        }

        /// <summary>
        /// Process a raw packet in it's entirety in the given byte array
        /// if deserialization is successful, then the packetdecoded event will be raised
        /// </summary>
        public MavlinkPacket ProcessPacketBytes(byte[] packetBytes)
        {
            //	 System ID	 1 - 255
            //	 Component ID	 0 - 255
            //	 Message ID	 0 - 255
            //6 to (n+6)	 Data	 (0 - 255) bytes
            var packet = new MavlinkPacket
                             {
                                 SystemId = packetBytes[0],
                                 ComponentId = packetBytes[1],
                                 Message = _encoder.Deserialize(packetBytes, 2)
                             };

            if (PacketReceived != null)
            {
                PacketReceived(this, packet);
            }

            return packet;
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
        public MavlinkFactory(bool isLittleEndian)
        {
            MavLink_Deserializer.SetDataIsLittleEndian(isLittleEndian);
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

    public delegate void PacketReceivedEventHandler(object sender, MavlinkPacket e);



    ///<summary>
    /// Represents a Mavlink message - both the message object itself
    /// and the identified sending party
    ///</summary>
    public class MavlinkPacket
    {
        /// <summary>
        /// The sender's system ID
        /// </summary>
        public int SystemId;

        /// <summary>
        /// The sender's component ID
        /// </summary>
        public int ComponentId;

        /// <summary>
        /// Object which is the mavlink message
        /// </summary>
        public object Message;
    }
}
