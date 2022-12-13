using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Models
{
    public class DebugConfigs
    {
        public bool DebugPowerSensorOnly { get; set; }
        public bool DebugRadio { get; set; }
        public bool DebugRelayOnly { get; set; }
        public bool DebugVoltmeterOnly { get; set; }
        public bool DebugThermometerOnly { get; set; }
    }
}
