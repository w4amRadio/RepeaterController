using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using Microsoft.Extensions.Logging;
using RepeaterController.Interfaces;
using UsbRelayTest;

namespace RepeaterController
{
    ///https://github.com/LibUsbDotNet/LibUsbDotNet
    ///for this version: https://github.com/LibUsbDotNet/LibUsbDotNet/tree/v2
    public class LibUsbRelayService : IRelayService
    {
        private ILogger _logger;

        private const int vendorId = 5824;      //0x16c0
        private const int productId = 1503;     //0x05df

        private UsbDevice device;
        private IUsbDevice wholeUsbDevice;

        public LibUsbRelayService(ILogger logger)
        {
            this._logger = logger;

            var deviceFinder = new UsbDeviceFinder(vendorId, productId);

            if (deviceFinder == null)
            {
                _logger.LogError("Could not find the specified device!");
            }
            else
            {
                _logger.LogDebug("Device Finder was not null...(good)");
            }

            var allDevices = UsbDevice.AllDevices;

            allDevices.ToList().ForEach(x => logger.LogDebug($"Found {x.FullName}, Name {x.Name}, VID: {x.Vid}, Path: {x.DevicePath}, Product ID: {x.Pid}"));

            var t = UsbDevice.AllLibUsbDevices;
            var w = UsbDevice.AllWinUsbDevices;
            //new UsbDeviceFinder()
            device = UsbDevice.OpenUsbDevice(deviceFinder);

            if (device == null)
            {
                _logger.LogError("Could not open USB Relay Device, device is listed as null and not found.");
                throw new Exception("Could not open USB Relay Device!");
            }

            if (!device.IsOpen)
            {
                device.Open();

                //is our device now actually open?

                _logger.LogDebug($"Device open status: {device.IsOpen}");
            }

            // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
            // it exposes an IUsbDevice interface. If not (WinUSB) the
            // 'wholeUsbDevice' variable will be null indicating this is 
            // an interface of a device; it does not require or support 
            // configuration and interface selection.
            wholeUsbDevice = device as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                logger.LogInformation("This is a 'WholeUsbDevice', it does require or support configuration and interface selection.");


                DisplayAllConfigs();

                byte configValue = 0;
                bool getConfigSuccess = wholeUsbDevice.GetConfiguration(out configValue);
                logger.LogDebug($"Get Config Success: {getConfigSuccess}, config value: {configValue}");

                for (int i = 0; i < 256; i++)
                {
                    logger.LogDebug($"Attempting configuration {i}...");
                    bool setConfigSuccess = wholeUsbDevice.SetConfiguration((byte) i);
                    logger.LogDebug($"Set Configuration Success: {setConfigSuccess}");
                    Thread.Sleep(3000);
                }

                //bool setConfigSuccess = wholeUsbDevice.SetConfiguration(1);     //tried 0, 1
                //logger.LogDebug($"Set Configuration Success: {setConfigSuccess}");

                bool claimInterfaceSuccess = wholeUsbDevice.ClaimInterface(0);  //tried 0, 2, 
                logger.LogDebug($"Claim interface success: {claimInterfaceSuccess}");
                wholeUsbDevice.ActiveEndpoints.ToList().ForEach(x => logger.LogDebug($"Endpoint: {x.EpNum}, Endpoint Info Descriptor: {x.EndpointInfo.Descriptor}"));
            }
            else
            {
                logger.LogInformation("This is NOT a 'WholeUsbDevice', it does NOT require or support configuration and interface selection.");
            }
        }

        public void TurnAllOff()
        {
            _logger.LogDebug($"LibUsbRelayService.TurnAllOff invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffAll;
            send[2] = (byte)RelayDevices.RelayAll;
            UsbEndpointWriter writer = wholeUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            wholeUsbDevice.ActiveEndpoints.ToList().ForEach(x => _logger.LogDebug($"Endpoint: {x.EpNum}, Endpoint Info Descriptor: {x.EndpointInfo.Descriptor}"));

            int bytesWritten;
            var errorCode = writer.Write(send, 30000, out bytesWritten);

            _logger.LogDebug($"Write executed with {bytesWritten} bytes written and returned error code: {errorCode}");
        }

        public void TurnAllOn()
        {
            _logger.LogDebug($"LibUsbRelayService.TurnAllOn invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnAll;
            send[2] = (byte)RelayDevices.RelayAll;
            UsbEndpointWriter writer = device.OpenEndpointWriter(WriteEndpointID.Ep01);
            int bytesWritten;
            var errorCode = writer.Write(send, 30000, out bytesWritten);

            wholeUsbDevice.ActiveEndpoints.ToList().ForEach(x => _logger.LogDebug($"Endpoint: {x.EpNum}, Endpoint Info Descriptor: {x.EndpointInfo.Descriptor}"));

            _logger.LogDebug($"Write executed with {bytesWritten} bytes written and returned error code: {errorCode}");
        }

        public void TurnOneOff()
        {
            _logger.LogDebug($"LibUsbRelayService.TurnOneOff invoked.");

            /*
             *  [23:07:50 INF] Endpoint: Length:7
                DescriptorType:Endpoint
                EndpointID:0x81
                Attributes:0x03
                MaxPacketSize:8
                Interval:20
                Refresh:0
                SynchAddress:0x00
            */
            byte[] send = new byte[7];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffSingle;
            send[2] = (byte)RelayDevices.Relay1;
            UsbEndpointWriter writer = wholeUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            wholeUsbDevice.ActiveEndpoints.ToList().ForEach(x => _logger.LogDebug($"Endpoint: {x.EpNum}, Endpoint Info Descriptor: {x.EndpointInfo.Descriptor}"));

            int bytesWritten;
            var errorCode = writer.Write(send, 30000, out bytesWritten);

            _logger.LogDebug($"Write executed with {bytesWritten} bytes written and returned error code: {errorCode}");
        }

        public void TurnOneOn()
        {
            if(wholeUsbDevice == null)
            {
                throw new Exception("WholeUsbDevice cannot be null!");
            }

            _logger.LogDebug($"LibUsbRelayService.TurnOneOn invoked.");
            byte[] send = new byte[7];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnSingle;
            send[2] = (byte)RelayDevices.Relay1;
            UsbEndpointWriter writer = wholeUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            wholeUsbDevice.ActiveEndpoints.ToList().ForEach(x => _logger.LogDebug($"Endpoint: {x.EpNum}, Endpoint Info Descriptor: {x.EndpointInfo.Descriptor}"));

            int bytesWritten;
            var errorCode = writer.Write(send, 30000, out bytesWritten);

            _logger.LogDebug($"Write executed with {bytesWritten} bytes written and returned error code: {errorCode}");
        }
         
        public void TurnTwoOff()
        {
            _logger.LogDebug($"LibUsbRelayService.TurnTwoOff invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffSingle;
            send[2] = (byte)RelayDevices.Relay2;
            UsbEndpointWriter writer = wholeUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            wholeUsbDevice.ActiveEndpoints.ToList().ForEach(x => _logger.LogDebug($"Endpoint: {x.EpNum}, Endpoint Info Descriptor: {x.EndpointInfo.Descriptor}"));

            int bytesWritten;
            var errorCode = writer.Write(send, 30000, out bytesWritten);

            _logger.LogDebug($"Write executed with {bytesWritten} bytes written and returned error code: {errorCode}");
        }

        public void TurnTwoOn()
        {
            _logger.LogDebug($"LibUsbRelayService.TurnTwoOn invoked.");
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnSingle;
            send[2] = (byte)RelayDevices.Relay2;
            UsbEndpointWriter writer = wholeUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            wholeUsbDevice.ActiveEndpoints.ToList().ForEach(x => _logger.LogDebug($"Endpoint: {x.EpNum}, Endpoint Info Descriptor: {x.EndpointInfo.Descriptor}"));

            int bytesWritten;
            var errorCode = writer.Write(send, 30000, out bytesWritten);

            _logger.LogDebug($"Write executed with {bytesWritten} bytes written and returned error code: {errorCode}");
        }

        private void DisplayAllConfigs()
        {
            _logger.LogInformation($"Device Path: {device.DevicePath}");
            _logger.LogInformation($"Driver Mode: {device.DriverMode}");

            device.ActiveEndpoints.ToList().ForEach(x => _logger.LogDebug($"Endpoint: {x.EpNum}, Endpoint Info Descriptor: {x.EndpointInfo.Descriptor}"));

            for (int iConfig = 0; iConfig < device.Configs.Count; iConfig++)
            {
                UsbConfigInfo configInfo = device.Configs[iConfig];
                _logger.LogInformation($"Device Config Info: {configInfo}");
                ReadOnlyCollection<UsbInterfaceInfo> interfaceList = configInfo.InterfaceInfoList;
                foreach(UsbInterfaceInfo interfaceInfo in interfaceList)
                {
                    _logger.LogInformation($"UsbInterfaceInfo: {interfaceInfo}");
                    _logger.LogInformation($"Endpoints:");
                    ReadOnlyCollection<UsbEndpointInfo> endpointList = interfaceInfo.EndpointInfoList;
                    foreach(UsbEndpointInfo usbEndpointInfo in endpointList)
                    {
                        _logger.LogInformation($"Endpoint: {usbEndpointInfo}");
                    }
                }
            }
        }
    }
}
