using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Models
{
    public class PowerMeasurement
    {
        public float MeasuredVoltage { get; set; }
        public float MeasuredCurrent { get; set; }
        public DateTime? TimeTag { get; set; }
    }
}
