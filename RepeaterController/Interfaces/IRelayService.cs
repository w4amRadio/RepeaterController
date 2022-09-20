using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Interfaces
{
    public interface IRelayService
    {
        void TurnAllOn();
        void TurnAllOff();
        void TurnOneOn();
        void TurnOneOff();
        void TurnTwoOn();
        void TurnTwoOff();
    }
}
