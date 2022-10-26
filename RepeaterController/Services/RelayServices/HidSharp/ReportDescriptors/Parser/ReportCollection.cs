using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp.ReportDescriptors.Parser
{
    public class ReportCollection : ReportMainItem
    {
        internal List<ReportMainItem> _children;

        public ReportCollection()
        {
            _children = new List<ReportMainItem>();
        }

        public void Clear()
        {
            List<ReportMainItem> children = new List<ReportMainItem>(_children);
            foreach (ReportMainItem child in children) { child.Parent = null; }
            Type = 0;
        }

        public IEnumerable<ReportMainItem> Children
        {
            get { foreach (ReportMainItem item in _children) { yield return item; } }
        }

        public IEnumerable<ReportCollection> Collections
        {
            get
            {
                foreach (ReportMainItem item in _children)
                { if (item is ReportCollection) { yield return (ReportCollection)item; } }
            }
        }

        public IEnumerable<ReportSegment> Segments
        {
            get
            {
                foreach (ReportMainItem item in _children)
                { if (item is ReportSegment) { yield return (ReportSegment)item; } }
            }
        }

        public CollectionType Type
        {
            get;
            set;
        }
    }
}
