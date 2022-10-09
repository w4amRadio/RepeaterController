/*
 * AUTHOR: David B. Dickey (KN4TEM) 
 * Written 8/31/2022 for CARC special project.
 * 
 * Example: https://chollinger.com/blog/2021/04/raspberry-pi-gardening-monitoring-a-vegetable-garden-using-a-raspberry-pi-part-1/
 * https://github.com/jeremylindsayni/Magellanic.I2c/blob/master/AbstractI2CDevice.cs
 * https://github.com/jeremylindsayni/Magellanic.Sensors.MCP9808/blob/master/MCP9808.cs
 * https://ww1.microchip.com/downloads/en/DeviceDoc/25095A.pdf
 */

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UsbRelayTest
{
    public class I2CThermometer
    {
        const byte mcp9808_thermometer_address = (byte)0x18;

        readonly byte[] readRegister = new byte[1] { (byte)MCP9808_Thermometer_Enums.ReadRegister };
        readonly byte[] manufacturerIdRegister = new byte[1] { (byte)MCP9808_Thermometer_Enums.ManufacturerIdRegister };
        readonly byte[] deviceIdRegister = new byte[1] { (byte)MCP9808_Thermometer_Enums.DeviceIdRegister };

        //private ReadOnlySpan<byte> temperatureAddress = 
        //    new ReadOnlySpan<byte>(new byte[] { (byte)MCP9808_Thermometer_Enums.ReadRegister }); 

        protected I2cDevice device;
        private ILogger _logger;

        public I2CThermometer(ILogger logger, bool troubleshootingMode)
        {
            if(logger == null)
            {
                throw new ArgumentNullException($"Logger passed in to I2CThermometer must not be null!");
            }

            _logger = logger; 

            //what is our i2cBus Id?
            I2cBus i2cbus = I2cBus.Create(1);
            _logger.LogDebug($"I2cBus created.");

            device = i2cbus.CreateDevice(mcp9808_thermometer_address);
            _logger.LogDebug("I2c device found.  Sleeping 300 ms.");
            Thread.Sleep(300);

            if (troubleshootingMode)
            {
                _logger.LogDebug($"ManufacturerID: {GetManufacturerId()}");
                _logger.LogDebug($"DeviceID: {GetDeviceId()}");

                /*
                device.WriteRead()
                for(int i = 1; i < 9; i++)
                {
                    byte[] t = new byte[1];
                    t[0] = (byte)i;
                    ReadOnlySpan<byte> tspan = new ReadOnlySpan<byte>(t);
                    device.Read(t);
                    _logger.LogDebug($"{i}: {tspan[0]}");
                }
                */
            }

            /*
            // Select configuration register
            // Continuous conversion mode, Power-up default
            byte[] config = new byte[3];
            config[0] = (byte) MCP9808_Thermometer_Enums.ConfigurationRegister; //the register we're writing to
            config[1] = 0x00;
            config[2] = 0x00;
            ReadOnlySpan<byte> configByteBuffer = new ReadOnlySpan<byte>(config);


            device.Write(configByteBuffer);   //but supposed to write to location 0x01
            _logger.LogDebug($"Power up written to i2c device for continuous conversion mode.");
            */

            /*
            //set resolution
            byte[] resolutionCodes = new byte[2];
            resolutionCodes[0] = (byte)MCP9808_Thermometer_Enums.ResolutionRegister;
            resolutionCodes[1] = (byte)0x03;
            ReadOnlySpan<byte> resolutionByteBuffer = new ReadOnlySpan<byte>(resolutionCodes);
            device.Write(resolutionByteBuffer);
            _logger.LogDebug("Resolution written to i2c temp device.");
            */

            Thread.Sleep(300);
        }

        public int GetManufacturerId()
        {
            byte[] readBuffer = new byte[2];
            device.WriteRead(manufacturerIdRegister, readBuffer);
            return BitConverter.ToUInt16(readBuffer, 0);
        }

        public int GetDeviceId()
        {
            byte[] deviceIdentifierBuffer = new byte[2];
            device.WriteRead(deviceIdRegister, deviceIdentifierBuffer);
            return BitConverter.ToUInt16(deviceIdentifierBuffer);
        }

        public double GetTemp(ThermometerConstants thermometerConstants)
        {
            _logger.LogDebug($"I2CThermometer.GetTemp invoked.");
            double retval = 0.0f;
            byte[] readBuffer = new byte[2];
            device.WriteRead(readRegister, readBuffer);

            _logger.LogDebug($"I2C temp device buffer read.");
            _logger.LogDebug($"I2C readBuffer[0]={readBuffer[0]}, readBuffer[1]={readBuffer[1]}");

            // Convert the data to 13-bits
            int temp = (readBuffer[0] & 0x1F) * 256 + (readBuffer[1] & 0xFF);
            if(temp > 4095)
            {
                temp -= 8192;
            }

            switch (thermometerConstants)
            {
                case ThermometerConstants.Celcius:
                    retval = temp * 0.0625;
                    break;
                case ThermometerConstants.Fahrenheit:
                    retval = (temp * 0.0625) * 1.8 + 32;
                    break;
                default:    //default is celsius
                    retval = temp * 0.0625;
                    break;
            }

            _logger.LogDebug($"I2C temp device read temperature as {retval} in {thermometerConstants.ToString()}");

            return retval;
        }
    }
}
