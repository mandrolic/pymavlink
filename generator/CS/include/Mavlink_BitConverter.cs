using System;
using System.Text;

namespace MavLink
{
    public static class FixedBufferUtil
    {
        public static unsafe void CopyToFixed(byte[] source, int sourceOffset, byte* target,
          int targetOffset, int count)
        {
            // If either offset, or the number of bytes to copy, is negative, you
            // cannot complete the copy.
            if ((sourceOffset < 0) || (targetOffset < 0) || (count < 0))
                throw new System.ArgumentException();

            if (source.Length - sourceOffset < count)
                throw new System.ArgumentException();

            byte* pt = target + targetOffset;

            // Copy the specified number of bytes from source to target.
            for (int i = 0; i < count; i++)
            {
                *pt = source[i + sourceOffset];
                pt++;
            }
        }

        public static unsafe void CopyToFixed(byte[] source, int sourceOffset, sbyte* target,
          int targetOffset, int count)
        {
            // If either offset, or the number of bytes to copy, is negative, you
            // cannot complete the copy.
            if ((sourceOffset < 0) || (targetOffset < 0) || (count < 0))
                throw new System.ArgumentException();

            if (source.Length - sourceOffset < count)
                throw new System.ArgumentException();

            sbyte* pt = target + targetOffset;

            // Copy the specified number of bytes from source to target.
            for (int i = 0; i < count; i++)
            {
                *pt = unchecked((sbyte)source[i + sourceOffset]);
                pt++;
            }
        }

        public static unsafe void CopyStringToFixed(string source, sbyte* dst, int dstOffset)
        {
            sbyte* pt = dst + dstOffset;

            // Copy the specified number of bytes from source to target.
            for (int i = 0; i < source.Length; i++)
            {
                *pt = unchecked((sbyte)source[i]);
                pt++;
            }
        }

        public static unsafe string CopyFixedToString(sbyte* source, int maxCount)
        {
            sbyte* pt = source;
            string outStr = string.Empty;
            // Copy the specified number of bytes from source to target.
            for (int i = 0; i < maxCount; i++)
            {
                outStr = outStr + *pt;
                pt++;
            }
            return outStr;
        }


        public static unsafe void CopyFromFixed(byte* source, int sourceOffset, byte[] target,
         int targetOffset, int count)
        {
            // todo
        }

        public static unsafe void CopyFromFixed(sbyte* source, int sourceOffset, byte[] target,
         int targetOffset, int count)
        {
            // todo
        }
    }

    /// <summary>
    /// Really simple and dumb converter from byte[] => primitive CLR types
    /// Designed to be (relatively...) host endianess independent (check floats though...)
    /// Mavlink packets are Big Endian <-- Actually I think they changed that for a new version. Better check.
    /// </summary>
    public class MavBitConverter
    {
        public UInt16 ToUInt16(byte[] value, int startIndex)
        {
            return (UInt16)(
                 value[1 + startIndex] << 0 |
                 value[0 + startIndex] << 8);
        }

        public Int16 ToInt16(byte[] value, int startIndex)
        {
            var val= unchecked ((Int16)(
                value[1 + startIndex] << 0 |
                value[0 + startIndex] << 8));

            return val;
        }


        public sbyte ToInt8(byte[] value, int startIndex)
        {
            return unchecked((sbyte)value[startIndex]);
        }

      

        public Int32 ToInt32(byte[] value,int startIndex)
        {
            return unchecked (
                value[3 + startIndex] << 0 |
                value[2 + startIndex] << 8 |
                value[1 + startIndex] << 16 |
                value[0 + startIndex] << 24);
        }

        public UInt32 ToUInt32(byte[] value, int startIndex)
        {
            return (UInt32)(
               value[3 + startIndex] << 0 |
               value[2 + startIndex] << 8 |
               value[1 + startIndex] << 16 |
               value[0 + startIndex] << 24);
        }

        public UInt64 ToUInt64(byte[] value, int startIndex)
        {
            return (UInt32)(
               value[7 + startIndex] << 0 |
               value[6 + startIndex] << 8 |
               value[5 + startIndex] << 16 |
               value[4 + startIndex] << 24 |
               value[3 + startIndex] << 32 |
               value[2 + startIndex] << 40 |
               value[1 + startIndex] << 48 |
               value[0 + startIndex] << 56);

        }

        public Int64 ToInt64(byte[] value, int startIndex)
        {
            return unchecked(
                value[7 + startIndex] << 0 |
                value[6 + startIndex] << 8 |
                value[5 + startIndex] << 16 |
                value[4 + startIndex] << 24 |
                value[3 + startIndex] << 32 |
                value[2 + startIndex] << 40 |
                value[1 + startIndex] << 48 |
                value[0 + startIndex] << 56);
        }

        public unsafe float ToSingle(byte[] value, int startIndex)
        {
            Int32 i = ToInt32(value, startIndex);
            return *(((float*)&i));
        }

        public byte[] GetBytes(sbyte value) 
        { 
            return new byte[1] 
                { 
                    (byte)value, 
                }; 
        }





        public unsafe byte[] GetBytes(double value)
        {
            ulong val = *((ulong*)&value);
            return GetBytes(val);
        }

        public unsafe byte[] GetBytes(float value) 
        {
            UInt32 val = *((UInt32*)&value);
            return GetBytes(val);
        }

      
       

        public byte[] GetBytes(UInt64 value) 
        {
            return new byte[8] 
            { 
                (byte)((value >> 56) & 0x000000FF),
                (byte)((value >> 48) & 0x000000FF),
                (byte)((value >> 40) & 0x000000FF),
                (byte)((value >> 32) & 0x000000FF),
                (byte)((value >> 24) & 0x000000FF),
                (byte)((value >> 16) & 0x000000FF),
                (byte)((value >> 8) & 0x000000FF),
                (byte)(value & 0x000000FF), 
            };
        }

         public byte[] GetBytes(Int64 value) {
            return new byte[8] 
            { 
                (byte)((value >> 56) & 0x000000FF),
                (byte)((value >> 48) & 0x000000FF),
                (byte)((value >> 40) & 0x000000FF),
                (byte)((value >> 32) & 0x000000FF),
                (byte)((value >> 24) & 0x000000FF),
                (byte)((value >> 16) & 0x000000FF),
                (byte)((value >> 8) & 0x000000FF),
                (byte)(value & 0x000000FF), 
            };
         }

     

        public byte[] GetBytes(UInt32 value)
        {
            return new byte[4] 
            { 
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF), 
            };
        }

        public byte[] GetBytes(UInt16 value) 
        { 
            return new byte[2] 
            { 
                (byte)(value >> 8), 
                (byte)(value & 0xFF) 
            }; 
        }

        public byte[] GetBytes(Int16 value) 
        {
            return new byte[2] 
            { 
                (byte)(value >> 8), 
                (byte)(value & 0xFF) 
            }; 
        }
    }
}
