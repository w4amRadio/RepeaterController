using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp
{
    class LinuxHidManager : HidManager
    {
        protected override object[] Refresh()
        {
            var paths = new List<string>(); 

            IntPtr udev = NativeMethods.udev_new();
            if (IntPtr.Zero != udev)
            {
                try
                {
                    IntPtr enumerate = NativeMethods.udev_enumerate_new(udev);
                    if (IntPtr.Zero != enumerate)
                    {
                        try
                        {
                            if (0 == NativeMethods.udev_enumerate_add_match_subsystem(enumerate, "hidraw") &&
                                0 == NativeMethods.udev_enumerate_scan_devices(enumerate))
                            {
                                IntPtr entry;
                                for (entry = NativeMethods.udev_enumerate_get_list_entry(enumerate); entry != IntPtr.Zero;
                                     entry = NativeMethods.udev_list_entry_get_next(entry))
                                {
                                    string syspath = NativeMethods.udev_list_entry_get_name(entry);
                                    if (syspath != null) { paths.Add(syspath); }
                                }
                            }
                        }
                        finally
                        {
                            NativeMethods.udev_enumerate_unref(enumerate);
                        }
                    }
                }
                finally
                {
                    NativeMethods.udev_unref(udev);
                }
            }

            return paths.Cast<object>().ToArray();
        }

        protected override bool TryCreateDevice(object key, out HidDevice device, out object creationState)
        {
            creationState = null;
            string syspath = (string)key; var hidDevice = new LinuxHidDevice(syspath);
            if (!hidDevice.GetInfo()) { device = null; return false; }
            device = hidDevice; return true;
        }

        protected override void CompleteDevice(object key, HidDevice device, object creationState)
        {

        }

        public override bool IsSupported
        {
            get
            {
                try
                {
                    string sysname; Version release; string machine;
                    if (NativeMethods.uname(out sysname, out release, out machine))
                    {
                        IntPtr udev = NativeMethods.udev_new();
                        if (IntPtr.Zero != udev)
                        {
                            NativeMethods.udev_unref(udev);

                            Console.WriteLine($"System Name: {sysname}, Version: {release}");
                            return sysname == "Linux" && release >= new Version(2, 6, 36);
                        }
                    }
                }
                catch
                {

                }
                finally
                {

                }

                Console.WriteLine("NOT SUPPORTED!");
                return false;
            }
        }
    }
}
