using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Models
{
    public class RepeaterReport
    {

        public double RadioOneTemperatureFahrenheit { get; set; }
        public double RadioTwoTemperatureFahrenheit { get; set; }
        public double OutletTemperatureFahrenheit { get; set; }
        public double MeasuredBatteryVoltage { get; set; }

        public bool InletFanOneActive { get; set; }
        public bool InletFanTwoActive { get; set; }
        public bool RadioOneFanActive { get; set; }
        public bool RadioTwoFanActive { get; set; }

        public double RadioOneFrequency { get; set; }
        public double RadioTwoFrequency { get; set; }

        public List<string> ActiveI2cAddresses { get; set; }

        public string RpiCpuUsage { get; set; }
        public string RpiMemUsage { get; set; }
        public string RpiDiskUsage { get; set; }

        public string RpiActiveProcesses { get; set; }
    }
}
