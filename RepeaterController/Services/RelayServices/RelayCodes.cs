/*
 * AUTHOR: David B. Dickey (KN4TEM) 
 * Written 8/31/2022 for CARC special project.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices
{
    public enum RelayCodes
    {
        OnAll = (byte)0xfe,
        OffAll = (byte)0xfc,
        OnSingle = (byte)0xff,
        OffSingle = (byte)0xfd
    }
}
