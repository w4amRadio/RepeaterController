using RepeaterController.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Interfaces
{
    public interface IPowerSensor
    {
        PowerMeasurement GetPowerMeasurement();
        bool PowerSensorHealthcheck();
    }
}
