using System;
using System.Text;

namespace MavLink
{
    public static class ByteArrayUtil
    {
        private static readonly MavBitConverter bitConverter = new MavBitConverter(); 

        

        public static byte[] ToChar(byte[] source, int sourceOffset, int size)
        {
            var bytes = new byte[size];

            for (int i = 0; i < size; i++)
                bytes[i] = source[i + sourceOffset];

            return bytes;
        }

        public static byte[] ToUInt8(byte[] source, int sourceOffset, int size)
        {
            var bytes = new byte[size];
            Array.Copy(source, sourceOffset, bytes, 0, size);
            return bytes;
        }

        public static sbyte[] ToInt8(byte[] source, int sourceOffset, int size)
        {
            var bytes = new sbyte[size];

            for (int i = 0; i < size; i++)
                bytes[i] = unchecked((sbyte)source[i + sourceOffset]);

            return bytes;
        }

        public static UInt16[] ToUInt16(byte[] source, int sourceOffset, int size)
        {
            var arr = new UInt16[size];
            for (int i = 0; i < size; i++)
                arr[i] = bitConverter.ToUInt16(source, sourceOffset + (i * sizeof (UInt16)));
            return arr;
        }

        public static Int16[] ToInt16(byte[] source, int sourceOffset, int size)
        {
            var arr = new Int16[size];
            for (int i = 0; i < size; i++)
                arr[i] = bitConverter.ToInt16(source, sourceOffset + (i * sizeof(Int16)));
            return arr;
        }

        public static UInt32[] ToUInt32(byte[] source, int sourceOffset, int size)
        {
            var arr = new UInt32[size];
            for (int i = 0; i < size; i++)
                arr[i] = bitConverter.ToUInt16(source, sourceOffset + (i * sizeof(UInt32)));
            return arr;
        }

        public static Int32[] ToInt32(byte[] source, int sourceOffset, int size)
        {
            var arr = new Int32[size];
            for (int i = 0; i < size; i++)
                arr[i] = bitConverter.ToInt16(source, sourceOffset + (i * sizeof(Int32)));
            return arr;
        }

        public static UInt64[] ToUInt64(byte[] source, int sourceOffset, int size)
        {
            var arr = new UInt64[size];
            for (int i = 0; i < size; i++)
                arr[i] = bitConverter.ToUInt16(source, sourceOffset + (i * sizeof(UInt64)));
            return arr;
        }

        public static Int64[] ToInt64(byte[] source, int sourceOffset, int size)
        {
            var arr = new Int64[size];
            for (int i = 0; i < size; i++)
                arr[i] = bitConverter.ToInt16(source, sourceOffset + (i * sizeof(Int64)));
            return arr;
        }

        public static Single[] ToSingle(byte[] source, int sourceOffset, int size)
        {
            var arr = new Single[size];
            for (int i = 0; i < size; i++)
                arr[i] = bitConverter.ToUInt16(source, sourceOffset + (i * sizeof(Single)));
            return arr;
        }

        public static Double[] ToDouble(byte[] source, int sourceOffset, int size)
        {
            var arr = new Double[size];
            for (int i = 0; i < size; i++)
                arr[i] = bitConverter.ToInt16(source, sourceOffset + (i * sizeof(Double)));
            return arr;
        }

        public static void ToByteArray(byte[] src, byte[] dst, int offset, int size)
        {
            int i;
            for (i = 0; i < size; i++)
                dst[offset + i] = src[i];
            while (i++ < size)
                dst[offset + i] = 0;
        }

        public static void ToByteArray(sbyte[] src, byte[] dst, int offset, int size)
        {
            int i;
            for (i = 0; i < size && i<src.Length; i++)
                dst[offset + i] = (byte)src[i];
            while (i++ < size)
                dst[offset + i] = 0;
        }

        public static void ToByteArray(UInt16[] src, byte[] dst, int offset, int size)
        {
            for (int i = 0; i < size && i < src.Length; i++)
                bitConverter.GetBytes(src[i], dst, offset + (i*sizeof (UInt16)));
        }

        public static void ToByteArray(Int16[] src, byte[] dst, int offset, int size)
        {
            for (int i = 0; i < size && i < src.Length; i++)
            {
                var bs = bitConverter.GetBytes(src[i]);
                dst[offset + i] = bs[0];
                dst[offset + i + 1] = bs[1];
            }
        }

        public static void ToByteArray(UInt32[] src, byte[] dst, int offset, int size)
        {
            for (int i = 0; i < size && i < src.Length; i++)
            {
                var bs = bitConverter.GetBytes(src[i]);
                dst[offset + i] = bs[0];
                dst[offset + i + 1] = bs[1];
                dst[offset + i + 2] = bs[2];
                dst[offset + i + 3] = bs[3];
            }
        }

        public static void ToByteArray(Int32[] src, byte[] dst, int offset, int size)
        {
            for (int i = 0; i < size && i < src.Length; i++)
            {
                var bs = bitConverter.GetBytes(src[i]);
                dst[offset + i] = bs[0];
                dst[offset + i + 1] = bs[1];
                dst[offset + i + 2] = bs[2];
                dst[offset + i + 3] = bs[3];
             
            }
        }

        public static void ToByteArray(Single[] src, byte[] dst, int offset, int size)
        {
            for (int i = 0; i < size && i < src.Length; i++)
            {
                var bs = bitConverter.GetBytes(src[i]);
                dst[offset + i] = bs[0];
                dst[offset + i + 1] = bs[1];
                dst[offset + i + 2] = bs[2];
                dst[offset + i + 3] = bs[3];

            }
        }

        public static void ToByteArray(UInt64[] src, byte[] dst, int offset, int size)
        {
            for (int i = 0; i < size && i < src.Length; i++)
            {
                var bs = bitConverter.GetBytes(src[i]);
                dst[offset + i] = bs[0];
                dst[offset + i + 1] = bs[1];
                dst[offset + i + 2] = bs[2];
                dst[offset + i + 3] = bs[3];
                dst[offset + i + 4] = bs[4];
                dst[offset + i + 5] = bs[5];
                dst[offset + i + 6] = bs[6];
                dst[offset + i + 7] = bs[7];
            }
        }

        public static void ToByteArray(Int64[] src, byte[] dst, int offset, int size)
        {
            for (int i = 0; i < size && i < src.Length; i++)
            {
                var bs = bitConverter.GetBytes(src[i]);
                dst[offset + i] = bs[0];
                dst[offset + i + 1] = bs[1];
                dst[offset + i + 2] = bs[2];
                dst[offset + i + 3] = bs[3];
                dst[offset + i + 4] = bs[4];
                dst[offset + i + 5] = bs[5];
                dst[offset + i + 6] = bs[6];
                dst[offset + i + 7] = bs[7];
            }
        }

        public static void ToByteArray(Double[] src, byte[] dst, int offset, int size)
        {
            for (int i = 0; i < size && i < src.Length; i++)
            {
                var bs = bitConverter.GetBytes(src[i]);
                dst[offset + i] = bs[0];
                dst[offset + i + 1] = bs[1];
                dst[offset + i + 2] = bs[2];
                dst[offset + i + 3] = bs[3];
                dst[offset + i + 4] = bs[4];
                dst[offset + i + 5] = bs[5];
                dst[offset + i + 6] = bs[6];
                dst[offset + i + 7] = bs[7];
            }
        }


        public static sbyte[] FromString(string str)
        {
            var encoding = new UTF8Encoding();
            var bytes =  encoding.GetBytes(str);

            var sbytes = new sbyte[bytes.Length];
            
            for (int i = 0; i < bytes.Length; i++)
                sbytes[i] = (sbyte) bytes[i];

            return sbytes;

            //return someParam.ToCharArray();
        }

        public static string ToString(sbyte[] sbytes)
        {
            var bytes = new byte[sbytes.Length];
            int i;
            for ( i = 0; i < bytes.Length && sbytes[i] != '\0'; i++)
                bytes[i] = (byte) sbytes[i];

            var bytesUntilNull = new byte[i];
            Array.Copy(bytes, bytesUntilNull, i);

            var encoding = new UTF8Encoding();

            return new string(encoding.GetChars(bytesUntilNull));
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
            return (UInt64)(
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

        // TODO: This is Host Endianess sensitive
        public unsafe Single ToSingle(byte[] value, int startIndex)
        {
            Int32 i = ToInt32(value, startIndex);
            return *(((Single*)&i));
        }

        // TODO: This is Host Endianess sensitive
        public unsafe Double ToDouble(byte[] value, int startIndex)
        {
            Int64 i = ToInt64(value, startIndex);
            return *(((Double*)&i));
        }

        // TODO: This is Host Endianess sensitive
        public unsafe byte[] GetBytes(double value)
        {
            ulong val = *((ulong*)&value);
            return GetBytes(val);
        }

        // TODO: This is Host Endianess sensitive
        public unsafe byte[] GetBytes(float value) 
        {
            UInt32 val = *((UInt32*)&value);
            return GetBytes(val);
        }

        public byte[] GetBytes(UInt64 value)
        {
            return new[] 
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

        public byte[] GetBytes(Int64 value)
        {
            return new[]
                       {
                           (byte) ((value >> 56) & 0x000000FF),
                           (byte) ((value >> 48) & 0x000000FF),
                           (byte) ((value >> 40) & 0x000000FF),
                           (byte) ((value >> 32) & 0x000000FF),
                           (byte) ((value >> 24) & 0x000000FF),
                           (byte) ((value >> 16) & 0x000000FF),
                           (byte) ((value >> 8) & 0x000000FF),
                           (byte) (value & 0x000000FF),
                       };
        }



        public void GetBytes(UInt32 value, byte[] dstArray, int offset)
        {
            dstArray[offset++] = (byte)(value >> 24);
            dstArray[offset++] = (byte)(value >> 16);
            dstArray[offset++] = (byte)(value >> 8);
            dstArray[offset] = (byte)(value & 0xFF);
        }

        public void GetBytes(Int16 value, byte[] dstArray, int offset)
        {

            dstArray[offset] = (byte) (value >> 8); 
            dstArray[offset + 1] = (byte) (value & 0xFF); 
        }


        public void GetBytes(Int32 value, byte[] dstArray, int offset)
        {

            dstArray[offset++] = (byte)(value >> 24);
            dstArray[offset++] = (byte)(value >> 16);
            dstArray[offset++] = (byte)(value >> 8);
            dstArray[offset] = (byte)(value & 0xFF);
        }

        public void GetBytes(UInt16 value, byte[] dstArray, int offset)
        {

            dstArray[offset] = (byte)(value >> 8);
            dstArray[offset + 1] = (byte)(value & 0xFF);
        }



        public byte[] GetBytes(Int16 value)
        {
            return new[] 
            { 
                (byte)(value >> 8), 
                (byte)(value & 0xFF) 
            };
        }

        public byte[] GetBytes(sbyte value)
        {
            return new byte[1] 
                { 
                    (byte)value, 
                };
        }
    }
}
