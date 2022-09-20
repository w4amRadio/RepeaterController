using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using RepeaterController.Interfaces;
using UsbRelayTest;

namespace RepeaterController
{
    public class LibUsbRelayService : IRelayService
    {
        private const int vendorId = 5824;
        private const int productId = 1503;

        private UsbDevice device;

        public LibUsbRelayService()
        {
            var deviceFinder = new UsbDeviceFinder(vendorId, productId);
            
            var allDevices = UsbDevice.AllDevices;
            var t = UsbDevice.AllLibUsbDevices;
            var w = UsbDevice.AllWinUsbDevices;
            //new UsbDeviceFinder()
            device = UsbDevice.OpenUsbDevice(deviceFinder);

            if(device == null)
            {
                throw new Exception("Could not open USB Relay Device!");
            }

            if (!device.IsOpen)
            {
                device.Open();
            }

        }

        public void TurnAllOff()
        {
            
        }

        public void TurnAllOn()
        {
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnAll;
            send[2] = (byte)RelayDevices.RelayAll;
            UsbEndpointWriter writer = device.OpenEndpointWriter(WriteEndpointID.Ep01);
            int bytesWritten;
            var errorCode = writer.Write(send, 30000, out bytesWritten);
        }

        public void TurnOneOff()
        {
            throw new NotImplementedException();
        }

        public void TurnOneOn()
        {
            throw new NotImplementedException();
        }

        public void TurnTwoOff()
        {
            throw new NotImplementedException();
        }

        public void TurnTwoOn()
        {
            throw new NotImplementedException();
        }
    }
}
