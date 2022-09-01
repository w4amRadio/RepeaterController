/*
 * AUTHOR: David B. Dickey (KN4TEM) 
 * Written 8/31/2022 for CARC special project.
 * 
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
        const byte mcp9808_thermometer = (byte)0x18;
        protected I2cDevice device;
        private ILogger _logger;

        public I2CThermometer(ILogger logger)
        {
            if(logger == null)
            {
                throw new ArgumentNullException($"Logger passed in to I2CThermometer must not be null!");
            }

            _logger = logger;

            //what is our i2cBus Id?
            I2cBus i2cbus = I2cBus.Create(0);
            _logger.LogDebug($"I2cBus created.");

            device = i2cbus.CreateDevice(mcp9808_thermometer);
            _logger.LogDebug("I2c device found.  Sleeping 300 ms.");
            Thread.Sleep(300);

            // Select configuration register
            // Continuous conversion mode, Power-up default
            byte[] config = new byte[3];
            config[0] = (byte) MCP9808_Thermometer_Enums.ConfigurationRegister; //the register we're writing to
            config[1] = 0x00;
            config[2] = 0x00;
            ReadOnlySpan<byte> configByteBuffer = new ReadOnlySpan<byte>(config);
            device.Write(configByteBuffer);   //but supposed to write to location 0x01
            _logger.LogDebug($"Power up written to i2c device for continuous conversion mode.");

            //set resolution
            byte[] resolutionCodes = new byte[2];
            resolutionCodes[0] = (byte)MCP9808_Thermometer_Enums.ResolutionRegister;
            resolutionCodes[1] = (byte)0x03;
            ReadOnlySpan<byte> resolutionByteBuffer = new ReadOnlySpan<byte>(resolutionCodes);
            device.Write(resolutionByteBuffer);
            _logger.LogDebug("Resolution written to i2c temp device.");

            Thread.Sleep(300);
        }

        public double GetTemp(ThermometerConstants thermometerConstants)
        {
            _logger.LogDebug($"I2CThermometer.GetTemp invoked.");
            double retval = 0.0f;
            byte[] readCommands = new byte[2];
            readCommands[0] = (byte)MCP9808_Thermometer_Enums.ReadRegister;
            readCommands[1] = 0x00;
            readCommands[2] = 0x00;
            Span<byte> readBuffer = new Span<byte>(readCommands);
            device.Read(readBuffer);
            _logger.LogDebug($"I2C temp device buffer read.");

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
