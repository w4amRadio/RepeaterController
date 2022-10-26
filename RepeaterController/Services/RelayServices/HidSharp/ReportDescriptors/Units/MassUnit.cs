using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Units
{
    /// <summary>
    /// Defines the possible units of mass.
    /// </summary>
    public enum MassUnit
    {
        /// <summary>
        /// The unit system has no unit of mass.
        /// </summary>
        None,

        /// <summary>
        /// The unit of mass is the gram (occurs in the SI Linear and Rotation unit systems).
        /// </summary>
        Gram,

        /// <summary>
        /// The unit of mass is the slug (occurs in the English Linear and Rotation unit systems).
        /// </summary>
        Slug
    }
}
