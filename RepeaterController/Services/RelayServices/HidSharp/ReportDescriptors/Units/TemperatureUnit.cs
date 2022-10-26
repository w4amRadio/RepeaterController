﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Units
{
    /// <summary>
    /// Defines the possible units of temperature.
    /// </summary>
    public enum TemperatureUnit
    {
        /// <summary>
        /// The unit system has no unit of temperature.
        /// </summary>
        None,

        /// <summary>
        /// The unit of temperature is Kelvin (occurs in SI Linear and Rotation unit systems).
        /// </summary>
        Kelvin,

        /// <summary>
        /// The unit of temperature is Fahrenheit (occurs in English Linear and Rotation unit systems).
        /// </summary>
        Fahrenheit
    }
}
