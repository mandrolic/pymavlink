using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MavlinkTest
{
    public class TestStream : Stream
    {
        public readonly List<byte[]> SentBytes = new List<byte[]>();

        public Queue<byte[]> RxQueue = new Queue<byte[]>();

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytes = RxQueue.Dequeue();
            if (bytes.Length > count)
            {
                Assert.Fail("Uhhh");
            }

            Array.Copy(bytes,0, buffer,offset,bytes.Length);
            return bytes.Length;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var bytes = new byte[count];
            Array.Copy(buffer,offset,bytes,0, count);
            SentBytes.Add(bytes);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}