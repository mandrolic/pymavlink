using System;

namespace MavLink
{
    /// <summary>
    /// converter from byte[] => primitive CLR types
    /// delegates to the .Net framework bitconverter for speed, and to avoid using unsafe pointer 
    /// casting for Silverlight.
    /// 
    /// Todo - what about endianess?
    /// </summary>
    internal class FrameworkBitConverter
    {
        public UInt16 ToUInt16(byte[] value, int startIndex)
        {
            return BitConverter.ToUInt16(value, startIndex);
        }

        public Int16 ToInt16(byte[] value, int startIndex)
        {
            return BitConverter.ToInt16(value, startIndex);
        }

        public sbyte ToInt8(byte[] value, int startIndex)
        {
            return unchecked((sbyte)value[startIndex]);
        }

        public Int32 ToInt32(byte[] value, int startIndex)
        {
            return BitConverter.ToInt32(value, startIndex);
        }

        public UInt32 ToUInt32(byte[] value, int startIndex)
        {
            return BitConverter.ToUInt32(value, startIndex);
        }

        public UInt64 ToUInt64(byte[] value, int startIndex)
        {
            return BitConverter.ToUInt64(value, startIndex);
        }

        public Int64 ToInt64(byte[] value, int startIndex)
        {
            return BitConverter.ToInt64(value, startIndex);
        }

        public Single ToSingle(byte[] value, int startIndex)
        {
            return BitConverter.ToSingle(value, startIndex);
        }

        public Double ToDouble(byte[] value, int startIndex)
        {
            return BitConverter.ToDouble(value, startIndex);
        }

        public void GetBytes(Double value, byte[] dst, int offset)
        {
            var bytes =  BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, dst, offset, bytes.Length);
            
        }
      
        public void GetBytes(Single value, byte[] dst, int offset)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, dst, offset, bytes.Length);
        }

        public void GetBytes(UInt64 value, byte[] dst, int offset)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, dst, offset, bytes.Length);
        }

        public void GetBytes(Int64 value, byte[] dst, int offset)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, dst, offset, bytes.Length);
        }

        public void GetBytes(UInt32 value, byte[] dst, int offset)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, dst, offset, bytes.Length);
        }

        public void GetBytes(Int16 value, byte[] dst, int offset)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, dst, offset, bytes.Length);
        }

        public void GetBytes(Int32 value, byte[] dst, int offset)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, dst, offset, bytes.Length);
        }

        public void GetBytes(UInt16 value, byte[] dst, int offset)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, dst, offset, bytes.Length);
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