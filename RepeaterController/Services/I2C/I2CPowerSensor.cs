using Microsoft.Extensions.Logging;
using RepeaterController.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using RepeaterController.Interfaces;

namespace RepeaterController.Services.I2C
{
    public class I2CPowerSensor : I2CSensor, IPowerSensor
    {

        const byte defaultPowerSensorAddress = 0x24;

        private bool _troubleshootingMode = false;

        public I2CPowerSensor(ILogger logger, byte i2cAddress, bool troubleshootingMode) : base(logger, i2cAddress)
        {
            if (logger == null)
            {
                throw new ArgumentNullException($"Logger passed in to I2CPowerSensor must not be null!");
            }

            this._troubleshootingMode = troubleshootingMode;
        }

        public bool PowerSensorHealthcheck()
        {
            byte[] readBuffer = new byte[12];
            device.WriteRead(Encoding.UTF8.GetBytes("healthcheck"), readBuffer);
            string response = Encoding.UTF8.GetString(readBuffer);

            return response.Equals("I am alive.", StringComparison.CurrentCultureIgnoreCase);
        }

        public PowerMeasurement GetPowerMeasurement()
        {
            byte[] readbuffer = new byte[256];
            device.Read(readbuffer);
            string response = Encoding.UTF8.GetString(readbuffer);

            if (_troubleshootingMode)
            {
                _logger.LogDebug($"Response from ArduinoNano: {response}");
            }

            var powerMeasurement = JsonSerializer.Deserialize<PowerMeasurement>(response);
            
            if(powerMeasurement is not null)
                powerMeasurement.TimeTag = DateTime.UtcNow;

            return powerMeasurement;
        }
    }
}
