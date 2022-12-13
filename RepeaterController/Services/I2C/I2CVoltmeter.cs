using Microsoft.Extensions.Logging;
using RepeaterController.Services.I2C;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterController.Services
{
    public class I2CVoltmeter : I2CSensor
    {

        //public const byte ina219_voltmeter_address = 0x40;
        //public const byte VoltageBusRegister = 0x02;

        readonly byte[] setCalibrationConfigCommand = new byte[1] { (byte)INA219_Register_Enums.ConfigRegister };
        readonly byte[] readBusVoltageRegisterCommand = new byte[1] { (byte)INA219_Register_Enums.BusVoltageRegister };
        readonly byte[] readShuntVoltageRegisterCommand = new byte[1] { (byte)INA219_Register_Enums.ShuntVoltageRegister };

        const double BUS_VOLTAGE_LSB = 4e-3;

        private UInt16 ina219_calValue = 0;
        private int ina219_currentDivider_mA = 0;
        private int ina219_powerMultiplier_mW = 0;

        public I2CVoltmeter(ILogger logger) : base(logger, (byte) INA219_Voltmeter_Enums.Default_I2C_Address)
        {
            SetCalibration_32V_2A();
        }

        private byte CalcAddress(byte INA_ADDR0, byte INA_ADDR1)
        {
            return (byte) ((byte)INA219_Voltmeter_Enums.Default_I2C_Address | (INA_ADDR0 != 0 ?  0x01 : 0x00) | (INA_ADDR1 != 0 ? 0x04 : 0x00)); 
        }

        public void SetCalibration_32V_2A()
        {
            // By default we use a pretty huge range for the input voltage,
            // which probably isn't the most appropriate choice for system
            // that don't use a lot of power.  But all of the calculations
            // are shown below if you want to change the settings.  You will
            // also need to change any relevant register settings, such as
            // setting the VBUS_MAX to 16V instead of 32V, etc.

            // VBUS_MAX = 32V             (Assumes 32V, can also be set to 16V)
            // VSHUNT_MAX = 0.32          (Assumes Gain 8, 320mV, can also be 0.16, 0.08,
            // 0.04) RSHUNT = 0.1               (Resistor value in ohms)

            // 1. Determine max possible current
            // MaxPossible_I = VSHUNT_MAX / RSHUNT
            // MaxPossible_I = 3.2A

            // 2. Determine max expected current
            // MaxExpected_I = 2.0A

            // 3. Calculate possible range of LSBs (Min = 15-bit, Max = 12-bit)
            // MinimumLSB = MaxExpected_I/32767
            // MinimumLSB = 0.000061              (61uA per bit)
            // MaximumLSB = MaxExpected_I/4096
            // MaximumLSB = 0,000488              (488uA per bit)

            // 4. Choose an LSB between the min and max values
            //    (Preferrably a roundish number close to MinLSB)
            // CurrentLSB = 0.0001 (100uA per bit)

            // 5. Compute the calibration register
            // Cal = trunc (0.04096 / (Current_LSB * RSHUNT))
            // Cal = 4096 (0x1000)

            ina219_calValue = 4096;

            // 6. Calculate the power LSB
            // PowerLSB = 20 * CurrentLSB
            // PowerLSB = 0.002 (2mW per bit)

            // 7. Compute the maximum current and shunt voltage values before overflow
            //
            // Max_Current = Current_LSB * 32767
            // Max_Current = 3.2767A before overflow
            //
            // If Max_Current > Max_Possible_I then
            //    Max_Current_Before_Overflow = MaxPossible_I
            // Else
            //    Max_Current_Before_Overflow = Max_Current
            // End If
            //
            // Max_ShuntVoltage = Max_Current_Before_Overflow * RSHUNT
            // Max_ShuntVoltage = 0.32V
            //
            // If Max_ShuntVoltage >= VSHUNT_MAX
            //    Max_ShuntVoltage_Before_Overflow = VSHUNT_MAX
            // Else
            //    Max_ShuntVoltage_Before_Overflow = Max_ShuntVoltage
            // End If

            // 8. Compute the Maximum Power
            // MaximumPower = Max_Current_Before_Overflow * VBUS_MAX
            // MaximumPower = 3.2 * 32V
            // MaximumPower = 102.4W

            // Set multipliers to convert raw current/power values
            ina219_currentDivider_mA = 10; // Current LSB = 100uA per bit (1000/100 = 10)
            ina219_powerMultiplier_mW = 2; // Power LSB = 1mW per bit (2/1)

            //set calibration register to 'Cal' calculated above
            WriteCalibration(ina219_calValue);

            // Set Config register to take into account the settings above
            UInt16 config = (ushort)INA219_Voltmeter_Bus_Range_Enums.Range_32V |
                (ushort)INA219_Gain_Enums.Gain_8_320MV | (ushort) INA219_ADC_Bus_Resolution_Enums._12Bit |
                (ushort)INA219_ADC_Shunt_Resolution_Enums._12BIT_1S_532US |
                (ushort)INA219_Config_Operating_Mode_Enums.ShuntAndBusVoltageContinuous;

            WriteConfiguration(config);
        }

        public void SetCalibration_32V_1A()
        {
            // By default we use a pretty huge range for the input voltage,
            // which probably isn't the most appropriate choice for system
            // that don't use a lot of power.  But all of the calculations
            // are shown below if you want to change the settings.  You will
            // also need to change any relevant register settings, such as
            // setting the VBUS_MAX to 16V instead of 32V, etc.

            // VBUS_MAX = 32V		(Assumes 32V, can also be set to 16V)
            // VSHUNT_MAX = 0.32	(Assumes Gain 8, 320mV, can also be 0.16, 0.08, 0.04)
            // RSHUNT = 0.1			(Resistor value in ohms)

            // 1. Determine max possible current
            // MaxPossible_I = VSHUNT_MAX / RSHUNT
            // MaxPossible_I = 3.2A

            // 2. Determine max expected current
            // MaxExpected_I = 1.0A

            // 3. Calculate possible range of LSBs (Min = 15-bit, Max = 12-bit)
            // MinimumLSB = MaxExpected_I/32767
            // MinimumLSB = 0.0000305             (30.5uA per bit)
            // MaximumLSB = MaxExpected_I/4096
            // MaximumLSB = 0.000244              (244uA per bit)

            // 4. Choose an LSB between the min and max values
            //    (Preferrably a roundish number close to MinLSB)
            // CurrentLSB = 0.0000400 (40uA per bit)

            // 5. Compute the calibration register
            // Cal = trunc (0.04096 / (Current_LSB * RSHUNT))
            // Cal = 10240 (0x2800)

            ina219_calValue = 0;


            // 6. Calculate the power LSB
            // PowerLSB = 20 * CurrentLSB
            // PowerLSB = 0.0008 (800uW per bit)

            // 7. Compute the maximum current and shunt voltage values before overflow
            //
            // Max_Current = Current_LSB * 32767
            // Max_Current = 1.31068A before overflow
            //
            // If Max_Current > Max_Possible_I then
            //    Max_Current_Before_Overflow = MaxPossible_I
            // Else
            //    Max_Current_Before_Overflow = Max_Current
            // End If
            //
            // ... In this case, we're good though since Max_Current is less than
            // MaxPossible_I
            //
            // Max_ShuntVoltage = Max_Current_Before_Overflow * RSHUNT
            // Max_ShuntVoltage = 0.131068V
            //
            // If Max_ShuntVoltage >= VSHUNT_MAX
            //    Max_ShuntVoltage_Before_Overflow = VSHUNT_MAX
            // Else
            //    Max_ShuntVoltage_Before_Overflow = Max_ShuntVoltage
            // End If

            // 8. Compute the Maximum Power
            // MaximumPower = Max_Current_Before_Overflow * VBUS_MAX
            // MaximumPower = 1.31068 * 32V
            // MaximumPower = 41.94176W

        }

        private void WriteConfiguration(UInt16 configurationvalue)
        {
            byte[] commandBytes = ConvertShortToByteArray(configurationvalue);

            List<byte> writeCommandToRegister = new List<byte>();
            writeCommandToRegister.Add((byte)INA219_Register_Enums.ConfigRegister);
            writeCommandToRegister.AddRange(commandBytes);
            device.Write(writeCommandToRegister.ToArray());

            //TODO: Ack?
        }

        private void WriteCalibration(UInt16 calibrationValue)
        {
            byte[] commandBytes = ConvertShortToByteArray(calibrationValue);

            List<byte> writeCommandToRegister = new List<byte>();
            writeCommandToRegister.Add((byte)INA219_Register_Enums.CalibrationRegister);
            writeCommandToRegister.AddRange(commandBytes);

            device.Write(writeCommandToRegister.ToArray());

            //TODO: Check ack?
        }

        private byte[] ConvertShortToByteArray(UInt16 unsigned16)
        {
            byte[] retval = new byte[2];
            byte mostSignificantBits = (byte)(unsigned16 >> 8);
            retval[0] = mostSignificantBits;
            byte leastSignificantBits = (byte)(unsigned16 & 0xFF);
            retval[1] = leastSignificantBits;

            return retval;
        }

        public double GetBusVoltage()
        {
            byte[] readBuffer = new byte[2];
            device.WriteRead(readBusVoltageRegisterCommand, readBuffer);
            int readnum = ((readBuffer[0] & 0xFF) << 8) | (readBuffer[1] & 0xFF);
            return readnum / 10.0;
            //return (readnum >> 3) * BUS_VOLTAGE_LSB;
        }

        public double GetBusVoltage2()
        {
            byte[] readBuffer = new byte[3];
            device.WriteRead(readBusVoltageRegisterCommand, readBuffer);
            base._logger.LogDebug($"ReadBuffer[0]: {readBuffer[0]}");
            base._logger.LogDebug($"ReadBuffer[1]: {readBuffer[1]}");
            base._logger.LogDebug($"ReadBuffer[2]: {readBuffer[2]}");

            Int16 value = (Int16)((readBuffer[1] << 8) | (readBuffer[2]));
            return (value >> 3) * 4e-3f;
        }

        public double GetShuntVoltage()
        {
            byte[] readBuffer = new byte[3];
            device.WriteRead(readShuntVoltageRegisterCommand, readBuffer);
            base._logger.LogDebug($"ReadBuffer[0]: {readBuffer[0]}");
            base._logger.LogDebug($"ReadBuffer[1]: {readBuffer[1]}");
            base._logger.LogDebug($"ReadBuffer[2]: {readBuffer[2]}");

            Int16 value = (Int16)((readBuffer[1] << 8) | readBuffer[2]);
            return value;

        }
    }
}
