/*
 * AUTHOR: David B. Dickey (KN4TEM) 
 * Written 8/31/2022 for CARC special project.
 * 
 */

using HidLibrary;
using Microsoft.Extensions.Logging;
using RepeaterController.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController
{
    //This only seems to work on Windows because RPi nuget can't resolve hid.dll 
    public class RelayService : IRelayService, IDisposable
    {

        private HidDevice _device;
        private ILogger _logger;

        private bool _oneIsOn = false;
        private bool _twoIsOn = false;

        private const int vendorId = 5824;
        private const int productId = 1503;

        public RelayService(ILogger logger)
        {
            if(logger == null)
            {
                throw new ArgumentNullException("Logger passed in to RelayService must not be null!");
            }
            
            _logger = logger;

            //vendor id 5824
            //product id 1503

            //or

            //vendor id 1893
            //product id 20496
            _device = HidDevices.Enumerate(vendorId, productId).FirstOrDefault();

            if (_device != null)
            {
                _logger.LogDebug($"HID Device located.  Opening device...");
                if (!_device.IsOpen)
                {
                    _device.OpenDevice();
                    _logger.LogDebug($"HID Device opened.");
                }

                //device.MonitorDeviceEvents = true;

                //turn on
                //send[1] = (byte)0xfe; 
                //send[1] = (byte)0xfc;
                //send[1] = (byte)0xff;       //on, this works


                //not permitted
                //await device.WriteAsync(send, 400);

                //HidReport report = device.ReadReport();

                //device.WriteFeatureData(send);


                //unable to do this
                //HidReport responseReport = device.ReadReport(300);

                //not permitted on this platform
                //await device.WriteReportAsync(report, 300); 

                //HidDeviceData hidDeviceData = devices.FirstOrDefault().;
            }
            else
            {
                _logger.LogWarning($"The Relay HID Device with vendor id {vendorId} and product id {productId} could not be found!");
                _logger.LogWarning($"Providing a list of available HID devices...");
                List<HidDevice> devices = HidDevices.Enumerate().ToList();
                foreach(HidDevice hidDevice in devices)
                {
                    _logger.LogWarning($"DEVICE:    {hidDevice.Description}");

                    byte[] mBytes = new byte[100];
                    hidDevice.ReadManufacturer(out mBytes);
                    _logger.LogWarning($"MANUFACTURER:  {Encoding.UTF8.GetString(mBytes)}");

                    byte[] pBytes = new byte[100];
                    hidDevice.ReadProduct(out pBytes);
                    logger.LogWarning($"PRODUCT: {Encoding.UTF8.GetString(pBytes)}");

                    //Console.WriteLine(hidDevice.ReadReport());
                }
            }
        }

        public void TurnAllOn()
        {
            _logger.LogDebug($"RelayService.TurnAllOn invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnAll;
            send[2] = (byte)RelayDevices.RelayAll;
            HidReport report = new HidReport(9, new HidDeviceData(send, HidDeviceData.ReadStatus.Success));

            bool result = _device.WriteFeatureData(send);
            _logger.LogDebug($"Result of device write was {result}");

            _oneIsOn = true;
            _twoIsOn = true;
        }

        public void TurnAllOff()
        {
            _logger.LogDebug($"RelayService.TurnAllOff invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffAll;
            send[2] = (byte)RelayDevices.RelayAll;

            HidReport report = new HidReport(9, new HidDeviceData(send, HidDeviceData.ReadStatus.Success));

            bool result = _device.WriteFeatureData(send);
            _logger.LogDebug($"Result of device write was {result}");

            _oneIsOn = false;
            _twoIsOn = false;
        }

        public void TurnOneOn()
        {
            _logger.LogDebug($"RelayService.TurnOneOn invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnSingle;           
            send[2] = (byte)RelayDevices.Relay1;

            HidReport report = new HidReport(9, new HidDeviceData(send, HidDeviceData.ReadStatus.Success));

            bool result = _device.WriteFeatureData(send);
            _logger.LogDebug($"Result of device write was {result}");

            _oneIsOn = true;
        }

        public void TurnOneOff()
        {
            _logger.LogDebug($"RelayService.TurnOneOff invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffSingle;
            send[2] = (byte)RelayDevices.Relay1;

            HidReport report = new HidReport(9, new HidDeviceData(send, HidDeviceData.ReadStatus.Success));

            bool result = _device.WriteFeatureData(send);
            _logger.LogDebug($"Result of device write was {result}");

            _oneIsOn = false;
        }

        public void TurnTwoOn()
        {
            _logger.LogDebug($"RelayService.TurnTwoOn invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnSingle;
            send[2] = (byte)RelayDevices.Relay2;

            HidReport report = new HidReport(9, new HidDeviceData(send, HidDeviceData.ReadStatus.Success));

            bool result = _device.WriteFeatureData(send);
            _logger.LogDebug($"Result of device write was {result}");

            _twoIsOn = true;
        }

        public void TurnTwoOff()
        {
            _logger.LogDebug($"RelayService.TurnTwoOff invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffSingle;
            send[2] = (byte)RelayDevices.Relay2;

            HidReport report = new HidReport(9, new HidDeviceData(send, HidDeviceData.ReadStatus.Success));

            bool result = _device.WriteFeatureData(send);
            _logger.LogDebug($"Result of device write was {result}");

            _twoIsOn = false;
        }

        public bool OneIsOn
        {
            get
            {
                return _oneIsOn;
            }
        }

        public bool TwoIsOn
        {
            get
            {
                return _twoIsOn;
            }
        }

        public void Dispose()
        {
            _device.CloseDevice();
            _logger.LogInformation($"HID Device closed.");
        }
    }
}
