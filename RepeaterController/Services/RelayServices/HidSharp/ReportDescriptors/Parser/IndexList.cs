using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    public class IndexList : IndexBase
    {
        public IndexList()
        {
            Indices = new List<IList<uint>>();
        }

        public override bool IndexFromValue(uint value, out int index)
        {
            for (int i = 0; i < Indices.Count; i++)
            {
                foreach (uint thisValue in Indices[i])
                {
                    if (thisValue == value) { index = i; return true; }
                }
            }

            return base.IndexFromValue(value, out index);
        }

        public override IEnumerable<uint> ValuesFromIndex(int index)
        {
            if (index < 0 || Indices.Count == 0) { yield break; }
            foreach (uint value in Indices[Math.Min(Count - 1, index)])
            { yield return value; }
        }

        public override int Count
        {
            get { return Indices.Count; }
        }

        public IList<IList<uint>> Indices
        {
            get;
            private set;
        }
    }
}
