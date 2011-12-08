using System;

namespace MavLink
{
    /// <summary>
    /// Really simple and dumb converter from byte[] => primitive CLR types
    /// Designed to be (relatively...) host endianess independent (check floats though...)
    /// </summary>
    internal class MavBitConverter
    {
        private bool _islittle = false;

        public void SetDataIsLittleEndian(bool islittle)
        {
            _islittle = islittle;
        }

        public UInt16 ToUInt16(byte[] value, int startIndex)
        {
            return _islittle
                       ? (UInt16) (
                                      value[0 + startIndex] << 0 |
                                      value[1 + startIndex] << 8)
                       : (UInt16) (
                                      value[1 + startIndex] << 0 |
                                      value[0 + startIndex] << 8);
        }

        public Int16 ToInt16(byte[] value, int startIndex)
        {
            return unchecked ((Int16)(
                                         value[1 + startIndex] << 0 |
                                         value[0 + startIndex] << 8));
        }

        public sbyte ToInt8(byte[] value, int startIndex)
        {

            return unchecked((sbyte)value[startIndex]);
        }

        public Int32 ToInt32(byte[] value,int startIndex)
        {
            if (_islittle)
                return unchecked(
                    value[0 + startIndex] << 0 |
                    value[1 + startIndex] << 8 |
                    value[2 + startIndex] << 16 |
                    value[3 + startIndex] << 24);
            else
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
            if (_islittle)
            return (UInt64)(
               value[0 + startIndex] << 0 |
               value[1 + startIndex] << 8 |
               value[2 + startIndex] << 16 |
               value[3 + startIndex] << 24 |
               value[4 + startIndex] << 32 |
               value[5 + startIndex] << 40 |
               value[6 + startIndex] << 48 |
               value[7 + startIndex] << 56);

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
            if(_islittle)
            return unchecked(
                value[0 + startIndex] << 0 |
                value[1 + startIndex] << 8 |
                value[2 + startIndex] << 16 |
                value[3 + startIndex] << 24 |
                value[4 + startIndex] << 32 |
                value[5 + startIndex] << 40 |
                value[6 + startIndex] << 48 |
                value[7 + startIndex] << 56);

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
        public unsafe void GetBytes(Double value, byte[] dst, int offset)
        {
            ulong val = *((ulong*)&value);
            GetBytes(val, dst, offset);
        }

        // TODO: This is Host Endianess sensitive
        public unsafe void GetBytes(Single value, byte[] dst, int offset) 
        {
            UInt32 val = *((UInt32*)&value);
            GetBytes(val, dst, offset);
        }

        public void GetBytes(UInt64 value, byte[] dstArray, int offset)
        {
            if (_islittle)
            {
                dstArray[offset++] = (byte)(value & 0xFF);
                dstArray[offset++] = (byte)((value >> 8) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 16) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 24) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 32) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 40) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 48) & 0x000000FF);
                dstArray[offset] = (byte)((value >> 56) & 0x000000FF);
            }
            else
            {
                dstArray[offset++] = (byte)((value >> 56) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 48) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 40) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 32) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 24) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 16) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 8) & 0x000000FF);
                dstArray[offset] = (byte)(value & 0xFF);
            }
        }

        public void GetBytes(Int64 value, byte[] dstArray, int offset)
        {
            if (_islittle)
            {
                dstArray[offset++] = (byte)(value & 0xFF);
                dstArray[offset++] = (byte)((value >> 8) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 16) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 24) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 32) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 40) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 48) & 0x000000FF);
                dstArray[offset] = (byte)((value >> 56) & 0x000000FF);
            }
            else
            {
                dstArray[offset++] = (byte)((value >> 56) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 48) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 40) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 32) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 24) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 16) & 0x000000FF);
                dstArray[offset++] = (byte)((value >> 8) & 0x000000FF);
                dstArray[offset] = (byte)(value & 0xFF);
            }
        }

        public void GetBytes(UInt32 value, byte[] dstArray, int offset)
        {
            if (_islittle)
            {
                dstArray[offset++] = (byte) (value & 0xFF);
                dstArray[offset++] = (byte) (value >> 8);
                dstArray[offset++] = (byte) (value >> 16);
                dstArray[offset++] = (byte) (value >> 24);
            }
            else
            {
                dstArray[offset++] = (byte) (value >> 24);
                dstArray[offset++] = (byte) (value >> 16);
                dstArray[offset++] = (byte) (value >> 8);
                dstArray[offset] = (byte) (value & 0xFF);
            }
        }

        public void GetBytes(Int16 value, byte[] dstArray, int offset)
        {
            if (_islittle)
            {
                dstArray[offset + 1] = (byte)(value >> 8);
                dstArray[offset] = (byte)(value & 0xFF); 
            }
            else
            {
                dstArray[offset] = (byte)(value >> 8);
                dstArray[offset + 1] = (byte)(value & 0xFF);    
            }
           
        }

        public void GetBytes(Int32 value, byte[] dstArray, int offset)
        {
            if (_islittle)
            {
                dstArray[offset++] = (byte)(value & 0xFF);
                dstArray[offset++] = (byte)(value >> 8);
                dstArray[offset++] = (byte)(value >> 16);
                dstArray[offset++] = (byte)(value >> 24);
            }
            else
            {
                dstArray[offset++] = (byte)(value >> 24);
                dstArray[offset++] = (byte)(value >> 16);
                dstArray[offset++] = (byte)(value >> 8);
                dstArray[offset] = (byte)(value & 0xFF);
            }
        }

        public void GetBytes(UInt16 value, byte[] dstArray, int offset)
        {
            if (_islittle)
            {
                dstArray[offset + 1] = (byte)(value >> 8);
                dstArray[offset] = (byte)(value & 0xFF);
            }
            else
            {
                dstArray[offset] = (byte)(value >> 8);
                dstArray[offset + 1] = (byte)(value & 0xFF);
            }
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
