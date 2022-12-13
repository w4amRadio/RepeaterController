using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.Radio
{
    public class GpioSerialComm
    {
        

        public GpioSerialComm()
        {
            SerialPort _serialPort = new SerialPort("COM1", 19200, Parity.None, 8, StopBits.One);
        }
    }
}
