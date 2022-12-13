using RepeaterController.Interfaces;
using RepeaterController.Models;
using RepeaterController.Services.RelayServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepeaterController.Services.Radio
{
    public class AnyTone778Service : IRelayRadioService, ISerialCommRadio
    {

        private LinuxFileHidDevice _linuxFileHidDevice;
        private RadioConfigs _radioConfigs;


        public bool MicIsActive { get; private set; }
        public bool FanIsActive { get; private set; }

        /// <summary>
        /// The 8 channel relay device will start back up with all relays in off when it loses power.
        /// </summary>
        /// <param name="hidDeviceService"></param>
        /// <param name="radioConfigs"></param>
        public AnyTone778Service(LinuxFileHidDevice hidDeviceService, RadioConfigs radioConfigs)
        {
            if (hidDeviceService == null)
            {
                throw new ArgumentNullException("LinuxFileHidDevice cannot be null!");
            }

            if(radioConfigs == null)
            {
                throw new ArgumentNullException("RadioConfigs must not be null!");
            }

            _linuxFileHidDevice = hidDeviceService;
            this._radioConfigs = radioConfigs;
        }

        public void ActivatePtt()
        {
            _linuxFileHidDevice.TurnOneOn();
            MicIsActive = true;
        }

        public void DeactivatePtt()
        {
            _linuxFileHidDevice.TurnOneOff();
            MicIsActive = false;
        }

        public void ChannelUp()
        {
            if (!MicIsActive)
            {
                _linuxFileHidDevice.TurnTwoOn();
                Thread.Sleep(_radioConfigs.ChannelChangePushButtonTimeMs);
                _linuxFileHidDevice.TurnTwoOff();
            }
        }

        public void ChannelDown()
        {
            if (!MicIsActive)
            {
                _linuxFileHidDevice.TurnThreeOn();
                Thread.Sleep(_radioConfigs.ChannelChangePushButtonTimeMs);
                _linuxFileHidDevice.TurnThreeOff();
            }
        }

        public void TurnOnfan()
        {
            if (!FanIsActive)
            {
                _linuxFileHidDevice.TurnFourOn();
                FanIsActive = true;
            }
        }

        public void TurnOffFan()
        {
            if (FanIsActive)
            {
                _linuxFileHidDevice.TurnFourOff();
                FanIsActive = false;
            }
        }

        public void PullState()
        {
            
        }
    }
}
