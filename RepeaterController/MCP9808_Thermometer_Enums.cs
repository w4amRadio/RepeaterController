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
    public enum MCP9808_Thermometer_Enums
    {
        ConfigurationRegister = (byte)0x01,
        ReadRegister = (byte)0x05,  //read two bytes from this register
        ManufacturerIdRegister = (byte)0x06,
        DeviceIdRegister = (byte)0x07,
        ResolutionRegister = (byte)0x08
        
    }

    public enum ThermometerConstants 
    {
        Celcius,
        Fahrenheit
    }

}
