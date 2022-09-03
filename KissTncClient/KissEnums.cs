using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KissTncClient
{
    public enum KissEnums
    {
        FEND = (byte) 0xC0,
        FESC = (byte) 0xDB,
        TFEND = (byte) 0xDC,
        TFESC = (byte) 0xDD
    }
}
