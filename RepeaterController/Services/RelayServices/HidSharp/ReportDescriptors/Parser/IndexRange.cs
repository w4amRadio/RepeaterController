using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    public class IndexRange : IndexBase
    {
        public IndexRange()
        {

        }

        public IndexRange(uint minimum, uint maximum)
        {
            Minimum = minimum; Maximum = maximum;
        }

        public override bool IndexFromValue(uint value, out int index)
        {
            if (value >= Minimum && value <= Maximum)
            {
                index = (int)(value - Minimum); return true;
            }

            return base.IndexFromValue(value, out index);
        }

        public override IEnumerable<uint> ValuesFromIndex(int index)
        {
            if (index < 0) { yield break; }
            yield return index >= Count ? Maximum : (uint)(Minimum + index);
        }

        public override int Count
        {
            get { return (int)(Maximum - Minimum + 1); }
        }

        public uint Minimum
        {
            get;
            set;
        }

        public uint Maximum
        {
            get;
            set;
        }
    }
}
