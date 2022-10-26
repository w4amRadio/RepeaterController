using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors
{
    public enum LocalItemTag : byte
    {
        Usage = 0,
        UsageMinimum,
        UsageMaximum,
        DesignatorIndex,
        DesignatorMinimum,
        DesignatorMaximum,
        StringIndex = 7,
        StringMinimum,
        StringMaximum,
        Delimiter
    }
}
