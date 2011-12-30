using System;
using System.IO;
using MavLink;

namespace MavUtil
{
    /// <summary>
    /// a MAVLink logfile reader/writer
    /// </summary>
    public class Mavlogfile : Mavfile
    {
        private readonly bool _noTimeStamps;

        public Mavlogfile(string fileName, byte sourceSystem = (byte)0xFF, bool noTimeStamps = false)
            : base(sourceSystem)
        {
            _noTimeStamps = noTimeStamps;
            //   if planner_format is None and self.filename.endswith(".tlog"):
            // self.planner_format = True

            SetStream(File.OpenRead(fileName));
        }


        public Mavlogfile(Stream logfiletream, byte sourceSystem = (byte)0xFF, bool noTimeStamps = false)
            : base(sourceSystem)
        {
            _noTimeStamps = noTimeStamps;
            //   if planner_format is None and self.filename.endswith(".tlog"):
            // self.planner_format = True

            _stream = logfiletream;
            SetStream(_stream);
        }

        protected override void preMessage()
        {
            //'''read timestamp if needed'''
            if (_noTimeStamps)
                return;

            if (_planner_format)
            {
                // Get the timestamp from the ascii planner format

                var tbuf = new byte[21];
                var bytesRead = _stream.Read(tbuf, 0, 21);

                if (bytesRead != 21 || tbuf[0] != '-' || tbuf[20] != ':')
                    // check for some perculliar format - is this true anymore?
                    throw new Exception("bad planner timestamp");
                var hnsec = Math.Pow(2.0, 63) + float.Parse(ByteArrayUtil.ToString(tbuf));

                var t = hnsec * 1.0e-7F;
                // convert to seconds
                t -= 719163L * 24 * 60 * 60;
                //convert to 1970 base
            }
            else
            {
                // Get the timestamp from 8 byte binary count of milliseconds
                var tbuf = new byte[8];
                var bytesRead = _stream.Read(tbuf, 0, 8);
                if (bytesRead == 8)
                {
                    Array.Reverse(tbuf); // assumes BE mavlink timestamp and LE host
                    var timeStampUs = BitConverter.ToUInt64(tbuf, 0);
                    var timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    try
                    {
                        // bogus global
                        _timestamp = timestamp.AddMilliseconds(timeStampUs / 1000);
                    }
                    catch (ArgumentOutOfRangeException exception)
                    {
                        // bad timestamp - what do here?
                    }

                }
            }
        }


        // add timestamp to message ( called directly after receive)
        protected override void postMessage(MavlinkPacket msg)
        {
            base.postMessage(msg);

            if (_noTimeStamps)
                msg.TimeStamp = DateTime.Now;
            else
                msg.TimeStamp = _timestamp;
            if (_planner_format)
                _stream.ReadByte(); // trailing newline

            _timestamp = msg.TimeStamp;
        }

    }
}