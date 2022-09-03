using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KissTncClient
{
    public enum KissCommandEnums
    {
        DataFrame = (byte) 0x00,
        TxDelay = (byte) 0x01,
        P = (byte) 0x02,
        SlotTime = (byte) 0x03,
        TxTail = (byte) 0x04,
        FullDuplex = (byte) 0x05,
        SetHardware = (byte) 0x06,
        MaxKissCommand = (byte) 0x07,
        Undefined = (byte) 0x80,
        MinKissCommand = (byte) 0xFD,
        LoopbackTest = (byte) 0xFE,
        Return = (byte) 0xFF
    }
}
