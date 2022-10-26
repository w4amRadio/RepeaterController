using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors
{
    public enum MainItemTag : byte
    {
        Input = 8,
        Output,
        Collection,
        Feature,
        EndCollection
    }
}
