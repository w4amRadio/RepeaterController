using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp
{
    /// <summary>
    /// Detects USB HID class devices connected to the system.
    /// </summary>
    [ComVisible(true), Guid("CD7CBD7D-7204-473c-AA2A-2B9622CFC6CC")]
    public class HidDeviceLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HidDeviceLoader"/> class.
        /// </summary>
        public HidDeviceLoader()
        {

        }

        /// <summary>
        /// Gets a list of connected USB devices.
        /// This overload is meant for Visual Basic 6 and COM clients.
        /// </summary>
        /// <returns>The device list.</returns>
        public IEnumerable GetDevicesVB()
        {
            return GetDevices();
        }

        /// <summary>
        /// Gets a list of connected USB devices.
        /// </summary>
        /// <returns>The device list.</returns>
        public IEnumerable<HidDevice> GetDevices()
        {
            return HidSelector.Instance.GetDevices();
        }

        /// <summary>
        /// Gets a list of connected USB devices, filtered by some criteria.
        /// </summary>
        /// <param name="vendorID">The vendor ID, or null to not filter by vendor ID.</param>
        /// <param name="productID">The product ID, or null to not filter by product ID.</param>
        /// <param name="productVersion">The product version, or null to not filter by product version.</param>
        /// <param name="serialNumber">The serial number, or null to not filter by serial number.</param>
        /// <returns>The filtered device list.</returns>
        public IEnumerable<HidDevice> GetDevices
            (int? vendorID = null, int? productID = null, int? productVersion = null, string serialNumber = null)
        {
            int vid = vendorID ?? -1, pid = productID ?? -1, ver = productVersion ?? -1;
            foreach (HidDevice hid in GetDevices())
            {
                if ((hid.VendorID == vendorID || vid < 0) &&
                    (hid.ProductID == productID || pid < 0) &&
                    (hid.ProductVersion == productVersion || ver < 0) &&
                    (hid.SerialNumber == serialNumber || string.IsNullOrEmpty(serialNumber)))
                {
                    yield return hid;
                }
            }
        }

        /// <summary>
        /// Gets the first connected USB device that matches specified criteria.
        /// </summary>
        /// <param name="vendorID">The vendor ID, or null to not filter by vendor ID.</param>
        /// <param name="productID">The product ID, or null to not filter by product ID.</param>
        /// <param name="productVersion">The product version, or null to not filter by product version.</param>
        /// <param name="serialNumber">The serial number, or null to not filter by serial number.</param>
        /// <returns>The device, or null if none was found.</returns>
        public HidDevice GetDeviceOrDefault
            (int? vendorID = null, int? productID = null, int? productVersion = null, string serialNumber = null)
        {
            return GetDevices(vendorID, productID, productVersion, serialNumber).FirstOrDefault();
        }
    }
}
