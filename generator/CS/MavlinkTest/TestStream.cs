using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MavlinkTest
{
    public class SizeQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly int maxSize;
        public SizeQueue(int maxSize) { this.maxSize = maxSize; }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                while (queue.Count >= maxSize)
                {
                    Monitor.Wait(queue);
                }
                queue.Enqueue(item);
                if (queue.Count == 1)
                {
                    // wake up any blocked dequeue
                    Monitor.PulseAll(queue);
                }
            }
        }
        public T Dequeue()
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                }
                T item = queue.Dequeue();
                if (queue.Count == maxSize - 1)
                {
                    // wake up any blocked enqueue
                    Monitor.PulseAll(queue);
                }
                return item;
            }
        }
    }

    public class TestStream : Stream
    {
        private byte[] leftovers;


        public readonly List<byte[]> SentBytes = new List<byte[]>();

        public SizeQueue<byte[]> RxQueue = new SizeQueue<byte[]>(10000);

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
            var bytes = leftovers ?? RxQueue.Dequeue();

            var bytesToCopy = Math.Min(count, bytes.Length);

            Array.Copy(bytes, 0, buffer, 0, bytesToCopy);

            if (bytes.Length > count)
            {
                // we have leftovers
                var leftoverlength = bytes.Length - bytesToCopy;
                leftovers = new byte[leftoverlength];
                Array.Copy(bytes, bytesToCopy,  leftovers, 0 ,leftoverlength);
            }
            else
            {
                leftovers = null;
            }

            return bytesToCopy;
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