using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp
{
    sealed class Throw
    {
        Throw()
        {

        }

        public static Throw If
        {
            get { return null; }
        }
    }

    static class ThrowExtensions
    {
        public static Throw Null<T>(this Throw self, T value, string paramName)
        {
            if (value == null) { throw new ArgumentNullException(paramName); }
            return null;
        }

        public static Throw OutOfRange<T>(this Throw self, IList<T> buffer, int offset, int count)
        {
            Throw.If.Null(buffer, "buffer");
            if (offset < 0 || offset > buffer.Count) { throw new ArgumentOutOfRangeException("offset"); }
            if (count < 0 || count > buffer.Count - offset) { throw new ArgumentOutOfRangeException("count"); }
            return null;
        }
    }
}
