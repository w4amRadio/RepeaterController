using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp
{
    sealed class Utf8Marshaler : ICustomMarshaler
    {
        bool _allocated; // workaround for Mono bug 4722

        public void CleanUpManagedData(object obj)
        {

        }

        public void CleanUpNativeData(IntPtr ptr)
        {
            if (IntPtr.Zero == ptr || !_allocated) { return; }
            Marshal.FreeHGlobal(ptr); _allocated = false;
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object obj)
        {
            string str = obj as string;
            if (str == null) { return IntPtr.Zero; }

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            Marshal.WriteByte(ptr, bytes.Length, 0);
            _allocated = true; return ptr;
        }

        public object MarshalNativeToManaged(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) { return null; }

            int length;
            for (length = 0; Marshal.ReadByte(ptr, length) != 0; length++) ;

            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            string str = Encoding.UTF8.GetString(bytes);
            return str;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new Utf8Marshaler();
        }
    }
}
