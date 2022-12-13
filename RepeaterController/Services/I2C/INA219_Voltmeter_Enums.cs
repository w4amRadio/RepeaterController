using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services.I2C
{
    /// <summary>
    /// Taken from https://github.com/adafruit/Adafruit_INA219/blob/master/Adafruit_INA219.h
    /// </summary>
    public enum INA219_Voltmeter_Enums
    {
        Read = 0x01,
        Default_I2C_Address = 0x40,         //INA219_ADDRESS  1000000 (A0+A1=GND)
        ConfigReset = 0x8000                //INA219_CONFIG_RESET 
    }

    public enum INA219_Mask_Enums
    {
        AdcResolutionMask = 0x0078,         //INA219_CONFIG_BADCRES_MASK 
        GainMask = 0x1800,                  //INA219_CONFIG_GAIN_MASK 
        BusVoltRangeMask = 0x2000,          //INA219_CONFIG_BVOLTAGERANGE_MASK 
        ConfigModeMask = 0x0007,            //INA219_CONFIG_MODE_MASK, Operating Mode Mask
    }

    public enum INA219_Gain_Enums
    {
        Gain_1_40MV = 0x0000,       //Gain 1, 40mV Range
        Gain_2_80MV = 0x0800,       //Gain 2, 80mV Range
        Gain_4_160MV = 0x1000,      //Gain 4, 160mV Range
        Gain_8_320MV = 0x1800,      //Gain 8, 320mV Range
    }

    public enum INA219_Register_Enums
    {
        ConfigRegister = 0x00,          //INA219_REG_CONFIG 
        ShuntVoltageRegister = 0x01,    //INA219_REG_SHUNTVOLTAGE 
        BusVoltageRegister = 0x02,      //INA219_REG_BUSVOLTAGE 
        PowerRegister = 0x03,           //INA219_REG_POWER 
        CurrentRegister = 0x04,         //INA219_REG_CURRENT 
        CalibrationRegister = 0x05     //INA219_REG_CALIBRATION
    }

    public enum INA219_Voltmeter_Bus_Range_Enums
    {
        Range_16V = 0x0000, //0V-16V
        Range_32V = 0x2000  //16V-32V
    }

    public enum INA219_ADC_Bus_Resolution_Enums
    {
        _9Bit = 0x0000,             //9 bit bus resolution = 0..511
        _10Bit = 0x0080,            //10 bit bus resolution = 0..1023
        _11Bit = 0x0100,            //INA219_CONFIG_BADCRES_11BIT 11 bit bus resolution = 0..2047
        _12Bit = 0x0180,            //INA219_CONFIG_BADCRES_12BIT 12 bit bus resolution = 0..4097
        _12BIT_2S_1060US = 0x0480,  //2 x 12-bit bus samples averaged together
        _12BIT_4S_2130US = 0x0500,  //4 x 12-bit bus samples averaged together
        _12BIT_8S_4260US = 0x0580,  //8 x 12-bit bus samples averaged together
        _12BIT_16S_8510US = 0x0600, //16 x 12-bit bus samples averaged together  
        _12BIT_32S_17MS = 0x0680,   //32 x 12-bit bus samples averaged together
        _12BIT_64S_34MS = 0x0700,   //64 x 12-bit bus samples averaged together
        _12BIT_128S_69MS = 0x0780   //128 x 12-bit bus samples averaged together
    }

    public enum INA219_ADC_Shunt_Resolution_Enums
    {
        _9BIT_1S_84US = 0x0000,         //INA219_CONFIG_SADCRES_9BIT_1S_84US  1 x 9-bit shunt sample
        _10BIT_1S_148US = 0x0008,       //INA219_CONFIG_SADCRES_10BIT_1S_148US 1 x 10-bit shunt sample
        _11BIT_1S_276US = 0x0010,       //INA219_CONFIG_SADCRES_11BIT_1S_276US  1 x 11-bit shunt sample
        _12BIT_1S_532US = 0x0018,       //INA219_CONFIG_SADCRES_12BIT_1S_532US 1 x 12-bit shunt sample
        _12BIT_2S_1060US = 0x0048,      //INA219_CONFIG_SADCRES_12BIT_2S_1060US 2 x 12-bit shunt samples averaged together
        _12BIT_4S_2130US = 0x0050,      //INA219_CONFIG_SADCRES_12BIT_4S_2130US 4 x 12-bit shunt samples averaged together
        _12BIT_8S_4260US = 0x0058,      //INA219_CONFIG_SADCRES_12BIT_8S_4260US 8 x 12-bit shunt samples averaged together
        _12BIT_16S_8510US = 0x0060,     //INA219_CONFIG_SADCRES_12BIT_16S_8510US 16 x 12-bit shunt samples averaged together
        _12BIT_32S_17MS = 0x0068,       //INA219_CONFIG_SADCRES_12BIT_32S_17MS 32 x 12-bit shunt samples averaged together
        _12BIT_64S_34MS = 0x0070,       //INA219_CONFIG_SADCRES_12BIT_64S_34MS 64 x 12-bit shunt samples averaged together
        _12BIT_128S_69MS = 0x0078       //INA219_CONFIG_SADCRES_12BIT_128S_69MS 128 x 12-bit shunt samples averaged together
    }

    public enum INA219_Config_Operating_Mode_Enums
    {
        PowerDown = 0x00,                   //INA219_CONFIG_MODE_POWERDOWN 
        ShuntVoltageTriggered = 0x01,       //INA219_CONFIG_MODE_SVOLT_TRIGGERED
        BusVoltageTriggered = 0x02,         //INA219_CONFIG_MODE_BVOLT_TRIGGERED 
        ShuntAndBusVoltageTriggered = 0x03, //INA219_CONFIG_MODE_SANDBVOLT_TRIGGERED 
        AdcOff = 0x04,                      //INA219_CONFIG_MODE_ADCOFF 
        ShuntVoltageContinuous = 0x05,      //INA219_CONFIG_MODE_SVOLT_CONTINUOUS 
        BusVoltageContinuous = 0x06,        //INA219_CONFIG_MODE_BVOLT_CONTINUOUS 
        ShuntAndBusVoltageContinuous = 0x07 //INA219_CONFIG_MODE_SANDBVOLT_CONTINUOUS 
    }
}
