using System.IO.Ports;

namespace MavUtil
{
    /// <summary>
    /// a serial mavlink port
    /// </summary>
    public class Mavserial : Mavfile
    {
        private int _baud;
        private bool _autoconnect;

        public Mavserial(int baud = 115200, bool autoreconnect = false, byte sourceSystem = (byte)0xFF, byte sourceComponent= (byte)0xFF)
            : base(sourceSystem, sourceComponent)
        {
            _baud = baud;
            _autoconnect = autoreconnect;

            //todo: 
            _stream = new SerialPort().BaseStream;
            SetStream(_stream);

        }

        protected override void preMessage()
        {
        }
    }
}