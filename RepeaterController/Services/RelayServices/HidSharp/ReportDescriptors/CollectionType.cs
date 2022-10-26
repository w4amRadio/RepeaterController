using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors
{
    public enum CollectionType : byte
    {
        Physical = 0,
        Application,
        Logical,
        Report,
        NamedArray,
        UsageSwitch,
        UsageModifier
    }
}
