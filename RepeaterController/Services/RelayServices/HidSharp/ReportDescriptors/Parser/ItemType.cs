using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    /// <summary>
    /// Describes the manner in which an item affects the descriptor.
    /// </summary>
    public enum ItemType : byte
    {
        /// <summary>
        /// Main items determine the report being described.
        /// For example, a main item switches between Input and Output reports.
        /// </summary>
        Main = 0,

        /// <summary>
        /// Global items affect all reports later in the descriptor.
        /// </summary>
        Global,

        /// <summary>
        /// Local items only affect the current report.
        /// </summary>
        Local,

        /// <summary>
        /// Long items use this type.
        /// </summary>
        Reserved
    }
}
