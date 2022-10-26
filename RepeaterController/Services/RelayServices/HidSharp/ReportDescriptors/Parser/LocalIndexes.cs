using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    public class LocalIndexes
    {
        IndexBase _designator, _string, _usage;

        public void Clear()
        {
            Designator = null; String = null; Usage = null;
        }

        public IndexBase Designator
        {
            get { return _designator ?? IndexBase.Unset; }
            set { _designator = value; }
        }

        public IndexBase String
        {
            get { return _string ?? IndexBase.Unset; }
            set { _string = value; }
        }

        public IndexBase Usage
        {
            get { return _usage ?? IndexBase.Unset; }
            set { _usage = value; }
        }
    }
}
