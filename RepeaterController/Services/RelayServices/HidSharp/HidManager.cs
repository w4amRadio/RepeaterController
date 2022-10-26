using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepeaterController.Services.RelayServices.HidSharp
{
    abstract class HidManager
    {
        Dictionary<object, HidDevice> _deviceList;
        object _syncRoot;

        protected HidManager()
        {
            _deviceList = new Dictionary<object, HidDevice>();
            _syncRoot = new object();
        }

        public virtual void Init()
        {

        }

        public virtual void Run()
        {
            while (true) { Thread.Sleep(Timeout.Infinite); }
        }

        internal void RunImpl(object readyEvent)
        {
            Init();
            ((ManualResetEvent)readyEvent).Set();
            Run();
        }

        public IEnumerable<HidDevice> GetDevices()
        {
            lock (SyncRoot)
            {
                object[] devices = Refresh();
                object[] additions = devices.Except(_deviceList.Keys).ToArray();
                object[] removals = _deviceList.Keys.Except(devices).ToArray();

                if (additions.Length > 0)
                {
                    int completedAdditions = 0;

                    foreach (object addition in additions)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(addition_ =>
                        {
                            HidDevice device; object creationState;
                            bool created = TryCreateDevice(addition_, out device, out creationState);

                            if (created)
                            {
                                // By not adding on failure, we'll end up retrying every time.
                                lock (_deviceList) { _deviceList.Add(addition_, device); }
                            }

                            lock (_deviceList)
                            {
                                completedAdditions++; Monitor.Pulse(_deviceList);
                            }

                            if (created)
                            {
                                CompleteDevice(addition_, device, creationState);
                            }
                        }), addition);
                    }

                    lock (_deviceList)
                    {
                        while (completedAdditions != additions.Length) { Monitor.Wait(_deviceList); }
                    }
                }

                foreach (object removal in removals)
                {
                    _deviceList.Remove(removal);
                }

                return _deviceList.Values.ToArray();
            }
        }

        protected abstract object[] Refresh();

        protected abstract bool TryCreateDevice(object key, out HidDevice device, out object creationState);

        protected abstract void CompleteDevice(object key, HidDevice device, object creationState);

        public abstract bool IsSupported
        {
            get;
        }

        protected object SyncRoot
        {
            get { return _syncRoot; }
        }
    }
}
