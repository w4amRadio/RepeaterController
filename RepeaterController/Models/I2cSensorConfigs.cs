using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Models
{
    public class I2cSensorConfigs
    {
        public byte FirstRadioTempAddress { get; set; }
        public byte SecondRadioTempAddress { get; set; }
        public byte VoltmeterAddress { get; set; }
    }
}
