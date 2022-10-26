using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp
{
    sealed class HidSelector
    {
        public static readonly HidManager Instance;
        static readonly Thread ManagerThread;

        static HidSelector()
        {
            foreach (var hidManager in new HidManager[]
                {
                    new LinuxHidManager()
                })
            {
                if (hidManager.IsSupported)
                {
                    var readyEvent = new ManualResetEvent(false);

                    Instance = hidManager;
                    ManagerThread = new Thread(Instance.RunImpl);
                    ManagerThread.IsBackground = true;
                    ManagerThread.Start(readyEvent);
                    readyEvent.WaitOne();

                    break;
                }
            }
        }
    }
}
