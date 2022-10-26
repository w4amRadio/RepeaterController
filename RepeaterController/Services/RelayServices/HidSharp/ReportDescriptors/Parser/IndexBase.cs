using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    public class IndexBase
    {
        public static readonly IndexBase Unset = new IndexBase();

        public bool ContainsValue(uint value)
        {
            int index; return IndexFromValue(value, out index);
        }

        public virtual bool IndexFromValue(uint value, out int index)
        {
            index = -1; return false;
        }

        public virtual IEnumerable<uint> ValuesFromIndex(int index)
        {
            yield break;
        }

        public virtual int Count
        {
            get { return 0; }
        }
    }
}
