using RepeaterController.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices
{


    /// <summary>
    /// For classes you will need to use [StructLayout(LayoutKind.Sequential)] decorator
    /// </summary>
    struct hidraw_devinfo
    {
        public uint bustype;
        public short vendor;
        public short product;
    };

    public class LinuxFileHidDevice : IRelayService, IDisposable
    {

        internal const uint _IOC_READ = 2;
        internal const int OPEN_READ_WRITE = 2;
        const string hid_device = "/dev/hidraw0"; 


        //#define HIDIOCGRAWINFO    _IOR('H', 0x03, struct hidraw_devinfo)


        public string iProduct { get; private set; }

        const byte _IOC_NRBITS      = 8;
        const byte _IOC_TYPEBITS    = 8;
        const byte _IOC_SIZEBITS    = 14;
        const byte _IOC_NRSHIFT     = 0;
        const byte _IOC_TYPESHIFT = (_IOC_NRSHIFT + _IOC_NRBITS);
        const byte _IOC_SIZESHIFT = (_IOC_TYPESHIFT + _IOC_TYPEBITS);
        const byte _IOC_DIRSHIFT = (_IOC_SIZESHIFT + _IOC_SIZEBITS);


        private int deviceHandle;

        public bool OneIsOn { get; private set; }
        public bool TwoIsOn { get; private set; }
        public bool ThreeIsOn { get; private set; }
        public bool FourIsOn { get; private set; }

        public LinuxFileHidDevice()
        {
            GetRawInfoAboutDevice();

            //File.OpenWrite("/dev/hidraw0");
            int open_success_code = Open(hid_device, OPEN_READ_WRITE);
            if (open_success_code < 0)
                throw new IOException(String.Format("Error opening bus '{0}': {1}", hid_device/*, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())*/));

            deviceHandle = open_success_code;

            Console.WriteLine($"findIor: {findIorForHidRawInfo((byte)'H', 6)}");

            Console.WriteLine($"findIoc: {findIoc(3, ((byte)'H'), 6, 3)}");
        }

        public void GetRawInfoAboutDevice()
        {
            hidraw_devinfo hidraw_Devinfo = new hidraw_devinfo();

            int result = 0;
            unsafe
            {
                int HIDIOCGRAWINFO = findIorForHidRawInfo((byte)'H', 6);

                result = Ioctl(deviceHandle, HIDIOCGRAWINFO, (IntPtr)(&hidraw_Devinfo));

                if(result < 0)
                {
                    throw new Exception($"An exception occured trying to read from the device with error code: {HIDIOCGRAWINFO}");
                }
                else
                {
                    Console.WriteLine($"Bus Type: {hidraw_Devinfo.bustype}");
                    Console.WriteLine($"Vendor: {hidraw_Devinfo.vendor}");
                    Console.WriteLine($"Product: {hidraw_Devinfo.product}");
                }
            }
        }

        public void WriteBytes(int numberOfCommands, byte[] bytes)
        {
            unsafe
            {
                int request = findIoc(3, ((byte)'H'), 6, numberOfCommands);

                fixed (byte* pointerToFirstElement = &bytes[0]) {

                    int ret = Ioctl(deviceHandle, request, (IntPtr)pointerToFirstElement);

                    if (ret < 0)
                        throw new IOException(String.Format("Error accessing address '{0}': {1}", request/*, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())*/));

                    ret = Write(deviceHandle, bytes, bytes.Length);

                    if (ret < 0)
                        throw new IOException(String.Format("Error writing to address '{0}': I2C transaction failed", request));
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // disposing managed resouces
            }

            if (deviceHandle != 0)
            {
                Close(new IntPtr(deviceHandle));
                deviceHandle = 0;
            }
        }

        //#define _IOC_NRBITS        8
        //#define _IOC_TYPEBITS        8

        /*
         * #ifndef _IOC_SIZEBITS
            # define _IOC_SIZEBITS        14
            #endif
        */

        //#define _IOC_NRSHIFT        0
        //#define _IOC_TYPESHIFT        (_IOC_NRSHIFT+_IOC_NRBITS)
        //#define _IOC_SIZESHIFT        (_IOC_TYPESHIFT+_IOC_TYPEBITS)
        //#define _IOC_DIRSHIFT        (_IOC_SIZESHIFT+_IOC_SIZEBITS)
        //_IOC_WRITE|_IOC_READ --> 3
        /*
         * #define _IOC(dir,type,nr,size) \
        (((dir)  << _IOC_DIRSHIFT) | \
         ((type) << _IOC_TYPESHIFT) | \
         ((nr)   << _IOC_NRSHIFT) | \
         ((size) << _IOC_SIZESHIFT))
        */

        private static int findIoc(int dir, int type, int nr, int size)
        {
            return (((dir) << _IOC_DIRSHIFT) | ((type) << _IOC_TYPESHIFT) | ((nr) << _IOC_NRSHIFT) | ((size) << _IOC_SIZESHIFT));
        }

        private static int findIorForHidRawInfo(int type, int nr)
        {
            unsafe
            {
                return findIoc((int)_IOC_READ, type, nr, sizeof(hidraw_devinfo));
            }
        }

        [DllImport("libc", EntryPoint = "open")]
        internal static extern int Open(string fileName, int mode);

        [DllImport("libc", EntryPoint = "close", SetLastError = true)]
        internal static extern int Close(IntPtr handle);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private extern static int Ioctl(int fd, int request, IntPtr data);

        [DllImport("libc", EntryPoint = "read", SetLastError = true)]
        internal static extern int Read(int handle, byte[] data, int length);

        [DllImport("libc", EntryPoint = "write", SetLastError = true)]
        internal static extern int Write(int handle, byte[] data, int length);

        public void TurnAllOn()
        {
            byte[] send = new byte[3];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnAll;
            send[2] = (byte)RelayDevices.RelayAll;
            WriteBytes(3, send);

            OneIsOn = true;
            TwoIsOn = true;
        }

        public void TurnAllOff()
        {
            byte[] send = new byte[3];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffAll;
            send[2] = (byte)RelayDevices.RelayAll;
            WriteBytes(3, send);
            OneIsOn = false;
            TwoIsOn = false;
        }



        public void TurnOneOn()
        {
            byte[] send = new byte[3];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnSingle;
            send[2] = (byte)RelayDevices.Relay1;
            WriteBytes(3, send);
            OneIsOn = true;
        }

        public void TurnOneOff()
        {
            byte[] send = new byte[3];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffSingle;
            send[2] = (byte)RelayDevices.Relay1;
            WriteBytes(3, send);
            OneIsOn = false;
        }


        #region Relay Two
        public void TurnTwoOn()
        {
            byte[] send = new byte[3];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnSingle;
            send[2] = (byte)RelayDevices.Relay2;
            WriteBytes(3, send);
            TwoIsOn = true;
        }

        public void TurnTwoOff()
        {
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffSingle;
            send[2] = (byte)RelayDevices.Relay2;
            WriteBytes(3, send);
            TwoIsOn = false;
        }

        #endregion

        #region Relay Three
        public void TurnThreeOn()
        {
            byte[] send = new byte[3];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnSingle;
            send[2] = (byte)RelayDevices.Relay3;
            WriteBytes(3, send);
            ThreeIsOn = true;
        }

        public void TurnThreeOff()
        {
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffSingle;
            send[2] = (byte)RelayDevices.Relay3;
            WriteBytes(3, send);
            ThreeIsOn = false;
        }

        #endregion

        #region Relay Four
        public void TurnFourOn()
        {
            byte[] send = new byte[3];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OnSingle;
            send[2] = (byte)RelayDevices.Relay4;
            WriteBytes(3, send);
            FourIsOn = true;
        }

        public void TurnFourOff()
        {
            byte[] send = new byte[9];
            send[0] = 0x0;
            send[1] = (byte)RelayCodes.OffSingle;
            send[2] = (byte)RelayDevices.Relay4;
            WriteBytes(3, send);
            FourIsOn = false;
        }

        #endregion
    }
}
