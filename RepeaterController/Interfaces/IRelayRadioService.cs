using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Interfaces
{
    public interface IRelayRadioService
    {
        void ActivatePtt();

        void DeactivatePtt();

        void ChannelUp();
        void ChannelDown();
        void TurnOnfan();
        void TurnOffFan();
    }
}
