using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    public enum GlobalItemTag : byte
    {
        UsagePage = 0,
        LogicalMinimum,
        LogicalMaximum,
        PhysicalMinimum,
        PhysicalMaximum,
        UnitExponent,
        Unit,
        ReportSize,
        ReportID,
        ReportCount,
        Push,
        Pop
    }
}
