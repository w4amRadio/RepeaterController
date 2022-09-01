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

namespace UsbRelayTest
{
    public enum RelayDevices
    {
        RelayAll = (byte) 0x00,
        Relay1 = (byte) 0x01,
        Relay2 = (byte) 0x02
    }
}
