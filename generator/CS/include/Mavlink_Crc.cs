using System;

namespace Mavlink
{
    /// <summary>
    /// Crc code copied/adapted from ardumega planner code
    /// </summary>
    internal static class Mavlink_Crc
    {
        const UInt16 X25_INIT_CRC = 0xffff;

        private static UInt16 CrcAccumulate(byte b, UInt16 crc)
        {
            unchecked
            {
                byte ch = (byte)(b ^ (byte)(crc & 0x00ff));
                ch = (byte)(ch ^ (ch << 4));
                return (UInt16)((crc >> 8) ^ (ch << 8) ^ (ch << 3) ^ (ch >> 4));
            }
        }


        // For a "message" of length bytes contained in the byte array
        // pointed to by buffer, calculate the CRC
        public static UInt16 Calculate(byte[] buffer, UInt16 start, UInt16 length)
        {
            UInt16 crcTmp = X25_INIT_CRC;

            for (int i = start; i < start + length; i++) 
                crcTmp = CrcAccumulate(buffer[i], crcTmp);

            return crcTmp;
        }
    }
}
