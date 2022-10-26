using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    public class ReportMainItem
    {
        ReportCollection _parent;

        public ReportMainItem()
        {
            Indexes = new LocalIndexes();
        }

        public LocalIndexes Indexes
        {
            get;
            private set;
        }

        public ReportCollection Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value) { return; }

                ReportCollection check = value;
                while (check != null && check != this) { check = check.Parent; }
                if (check == this) { throw new ArgumentException("Can't set up a loop."); }

                if (_parent != null) { _parent._children.Remove(this); }
                _parent = value;
                if (_parent != null) { _parent._children.Add(this); }
            }
        }
    }
}
