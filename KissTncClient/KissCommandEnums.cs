using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KissTncClient
{
    public enum KissCommandEnums
    {
        DataFrame = 0x00,
        TxDelay = 0x01,
        P = 0x02,
        SlotTime = 0x03,
        TxTail = 0x04,
        FullDuplex = 0x05,
        SetHardware = 0x06,
        MaxKissCommand = 0x07,
        Undefined = 0x80,
        MinKissCommand = 0xFD,
        LoopbackTest = 0xFE,
        Return = 0xFF
    }
}
