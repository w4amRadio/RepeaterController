using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    public delegate void ReportScanCallback
        (byte[] buffer, int bitOffset, ReportSegment segment);

    /// <summary>
    /// Reads and writes HID reports.
    /// </summary>
    public class Report
    {
        internal List<ReportSegment> _segments;

        /// <summary>
        /// Initializes a new instance of the <see cref="Report"/> class.
        /// </summary>
        public Report()
        {
            _segments = new List<ReportSegment>();
        }

        /// <summary>
        /// Resets the instance to its initial state.
        /// </summary>
        public void Clear()
        {
            List<ReportSegment> segments = new List<ReportSegment>(_segments);
            foreach (ReportSegment segment in segments) { segment.Report = null; }
            ID = 0; Type = 0;
        }

        /// <summary>
        /// Reads a HID report, calling back a provided function for each segment.
        /// </summary>
        /// <param name="buffer">The buffer containing the report.</param>
        /// <param name="offset">The offset to begin reading the report at.</param>
        /// <param name="callback">
        ///     This callback will be called for each report segment.
        ///     Use this to read every value you need.
        /// </param>
        public void Scan(byte[] buffer, int offset, ReportScanCallback callback)
        {
            int bitOffset = offset * 8;

            foreach (ReportSegment segment in Segments)
            {
                callback(buffer, bitOffset, segment);
                bitOffset += segment.BitCount;
            }
        }

        /// <summary>
        /// Writes a HID report, calling back a provided function for each segment.
        /// </summary>
        /// <param name="callback">
        ///     This callback will be called for each report segment.
        ///     Write to each segment to write a complete HID report.
        /// </param>
        public byte[] Write(ReportScanCallback callback)
        {
            byte[] buffer = new byte[1 + Length];
            buffer[0] = ID; Scan(buffer, 1, callback);
            return buffer;
        }

        /// <summary>
        /// The Report ID.
        /// </summary>
        public byte ID
        {
            get;
            set;
        }

        /// <summary>
        /// The length of this particular report.
        /// The Report ID is not included in this length.
        /// </summary>
        public int Length
        {
            get
            {
                int bits = 0;
                foreach (ReportSegment segment in _segments) { bits += segment.BitCount; }
                return (bits + 7) / 8;
            }
        }

        public IEnumerable<ReportSegment> Segments
        {
            get { foreach (ReportSegment segment in _segments) { yield return segment; } }
        }

        public ReportType Type
        {
            get;
            set;
        }
    }
}
