using RepeaterController.Services.RelayServices.HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices
{
    public class HidSharpTestService
    {
        public HidSharpTestService()
        {
            HidDeviceLoader loader = new HidDeviceLoader();
            var device = loader.GetDevices(5824, 1503).FirstOrDefault();

            if (device == null) { Console.WriteLine("Failed to open device."); Environment.Exit(1); }

            HidStream stream;
            if (!device.TryOpen(out stream)) { Console.WriteLine("Failed to open device."); Environment.Exit(2); }

            using (stream)
            {
                int n = 0;
                while (true)
                {
                    var bytes = new byte[device.MaxInputReportLength];
                    int count;

                    try
                    {
                        count = stream.Read(bytes, 0, bytes.Length);
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Read timed out.");
                        continue;
                    }
                }
            }
        }
    }
}
