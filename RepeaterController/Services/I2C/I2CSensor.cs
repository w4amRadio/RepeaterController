using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services
{
    public class I2CSensor
    {

        protected I2cDevice device;
        protected readonly I2cBus _i2CBus;
        protected ILogger _logger;

        public I2CSensor(ILogger logger, byte device_address)
        {
            if (logger == null)
            {
                throw new ArgumentNullException($"Logger passed in to I2CSensor must not be null!");
            }

            _logger = logger;

            //what is our i2cBus Id?
            _i2CBus = I2cBus.Create(1);
            _logger.LogDebug($"I2cBus created.");

            device = _i2CBus.CreateDevice(device_address);
            _logger.LogDebug("I2C device found.  Sleeping 300 ms.");
        }
    }
}
