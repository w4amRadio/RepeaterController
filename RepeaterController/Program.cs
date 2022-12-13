/*
 * AUTHOR: David B. Dickey (KN4TEM) 
 * Written 8/31/2022 for CARC special project.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using HidLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RepeaterController;
using RepeaterController.Interfaces;
using RepeaterController.Models;
using Serilog;
using Serilog.Extensions.Logging;
using RepeaterController.Extensions;
using RepeaterController.Services.RelayServices;
using RepeaterController.Services.Radio;
using RepeaterController.Services;
using RepeaterController.Services.I2C;
using System.Text.Json;

namespace RepeaterController
{
    public class Program
    {

        const int tempCheckInterval = 30000;        //TODO: make configurable at run time

        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var serilogLogger = Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var serilogFactory = new SerilogLoggerFactory(serilogLogger);

            Microsoft.Extensions.Logging.ILogger logger = serilogFactory.CreateLogger<Program>();



            //Start up our tnc to talk to direwolf and wait for command authority commands


            //Measure the temperature every 30 seconds, if the temp is above a certain value, turn the fan on
            //if the temperature is below a certain value, turn the fans off

            logger.LogDebug($"Instantiating timer for temp checking with interval of {tempCheckInterval/1000} seconds.");


            var debugConfigs = config.GetSection($"Settings:{nameof(DebugConfigs)}")?.Get<DebugConfigs>();
            var radioConfigs = config.GetSection($"Settings:{nameof(RadioConfigs)}")?.Get<RadioConfigs>();
            var i2cSensorConfigs = config.GetSection($"Settings:{nameof(I2cSensorConfigs)}")?.Get<I2cSensorConfigs>();

            if (debugConfigs.DebugPowerSensorOnly)
            {
                IPowerSensor powerSensor = new I2CPowerSensor(serilogFactory.CreateLogger<I2CPowerSensor>(), 0x24, true);
                logger.LogDebug($"Power sensor logger returned value of {powerSensor.PowerSensorHealthcheck()}");
            }

            if (debugConfigs.DebugRadio)
            {
                //for this test key mic, leave on for 10seconds, turn off mic, then change channel up twice, then change channel down twice
                IRelayRadioService relayRadio = new AnyTone778Service(new LinuxFileHidDevice(), radioConfigs);
                relayRadio.ActivatePtt();
                Thread.Sleep(10000);
                relayRadio.DeactivatePtt();

                relayRadio.ChannelUp();
                Thread.Sleep(3000);
                relayRadio.ChannelUp();

                Thread.Sleep(10000);
                relayRadio.ChannelDown();
                Thread.Sleep(3000);
                relayRadio.ChannelDown();

                Thread.Sleep(10000);
                relayRadio.TurnOnfan();
                Thread.Sleep(15000);
                relayRadio.TurnOffFan();
            }

            if (debugConfigs.DebugRelayOnly)
            {
                //IRelayService relayService = new RelayService(serilogFactory.CreateLogger<RelayService>());
                //IRelayService relayService = new LibUsbRelayService(serilogFactory.CreateLogger<LibUsbRelayService>());
                //HidSharpTestService hidSharpTestService = new HidSharpTestService();
                IRelayService relayService = new LinuxFileHidDevice();

                //turn on 1
                relayService.TurnOneOn();
                Thread.Sleep(10000);
                relayService.TurnOneOff();
                Thread.Sleep(10000);
                relayService.TurnTwoOn();
                Thread.Sleep(10000);
                relayService.TurnTwoOff();
                Thread.Sleep(10000);
                //turn on for 10 seconds
                relayService.TurnAllOn();
                Thread.Sleep(10000);
                relayService.TurnAllOff();
            }

            if (debugConfigs.DebugThermometerOnly)
            {
                I2CThermometer vhfRadioThermometer = 
                    new I2CThermometer(serilogFactory.CreateLogger<I2CThermometer>(), i2cSensorConfigs.FirstRadioTempAddress, true);

                I2CThermometer uhfRadioThermometer =
                    new I2CThermometer(serilogFactory.CreateLogger<I2CThermometer>(), i2cSensorConfigs.SecondRadioTempAddress, true);

                for(int i = 0; i < 50; i++)
                {
                    logger.LogDebug($"Measured temperature of the vhf radio is {vhfRadioThermometer.GetTemp(ThermometerConstants.Fahrenheit)} Fahrenheit.");
                    logger.LogDebug($"Measured temperature of the uhf radio is {uhfRadioThermometer.GetTemp(ThermometerConstants.Fahrenheit)} Fahrenheit.");
                    Thread.Sleep(3000);
                }
            }

            if (debugConfigs.DebugVoltmeterOnly)
            {
                I2CVoltmeter i2CVoltmeter = new I2CVoltmeter(serilogFactory.CreateLogger<I2CVoltmeter>());

                for(int i = 0; i < 20; i++)
                {
                    logger.LogDebug($"Measured voltage is {i2CVoltmeter.GetBusVoltage()} Volts.");
                    logger.LogDebug($"GetVoltage2 is {i2CVoltmeter.GetBusVoltage2()} Volts.");
                    logger.LogDebug($"GetShuntVoltage is {i2CVoltmeter.GetShuntVoltage()} Volts.");
                    Thread.Sleep(1000);
                } 
            }


            if(!debugConfigs.DebugRadio && !debugConfigs.DebugRelayOnly && !debugConfigs.DebugThermometerOnly && !debugConfigs.DebugVoltmeterOnly)
            {
                using (System.Timers.Timer tempTimer = new System.Timers.Timer(interval: tempCheckInterval))
                {  
                    Random rand = new Random(23984256);
                    I2CThermometer thermometer = new I2CThermometer(serilogFactory.CreateLogger<I2CThermometer>(), false);
                    //RelayService relayService = new RelayService(serilogFactory.CreateLogger<RelayService>());
                    IRelayService relayService = new LinuxFileHidDevice();
                    tempTimer.Elapsed += (sender, e) => TemperatureCheckHandler(logger, rand, thermometer, relayService);
                    tempTimer.Start();

                    while (true)
                    {
                        Thread.Sleep(3000);
                    }
                }
            }
        }

        private static void PowerMeasurementHandler(Microsoft.Extensions.Logging.ILogger logger, IPowerSensor powerSensor)
        {
            var powerMeasurement = powerSensor.GetPowerMeasurement();
            logger.LogInformation($"Power Measurement Reading: {JsonSerializer.Serialize(powerMeasurement)}");
            logger.LogInformation($"Power consumption at time {powerMeasurement.TimeTag} UTC is {powerMeasurement.MeasuredVoltage * powerMeasurement.MeasuredCurrent} watts.");
        }

        /// <summary>
        /// TODO: Also maybe take into account battery state and user input if we want to keep the fans off
        /// </summary>
        /// <param name="thermometer"></param>
        /// <param name="relayService"></param>
        private static void TemperatureCheckHandler(
            Microsoft.Extensions.Logging.ILogger logger,
            Random rand,
            I2CThermometer thermometer,
            IRelayService relayService)
        {
            double measuredTemp = thermometer.GetTemp(ThermometerConstants.Fahrenheit);
            logger.LogDebug($"Measured temperature is {measuredTemp} Fahrenheit.");
            
            if(measuredTemp <= 65)
            {
                logger.LogDebug($"Temperature is less than 65 workflow.");
                if(relayService.OneIsOn || relayService.TwoIsOn)
                {
                    logger.LogInformation($"Temperature is 65 or less, and at least one fan is on, turning all fans off.");
                    relayService.TurnAllOff();
                }
            } 
            else if(measuredTemp.BetweenInclusive(66, 75))
            {
                logger.LogDebug($"Temperature is between 66 and 75 workflow.");
                if (!relayService.OneIsOn && !relayService.TwoIsOn)
                {
                    //try to run the fans the same amount of time
                    if((int) rand.NextDouble() % 2 == 0)
                    {
                        logger.LogInformation($"Temperature is between 66 and 75, neither fan is on. Turning on fan one.");
                        relayService.TurnOneOn();
                    }
                    else
                    {
                        logger.LogInformation($"Temperature is between 66 and 75, neither fan is on. Turning on fan two.");
                        relayService.TurnTwoOn();
                    }
                    
                }
            }
            else if(measuredTemp > 75)
            {
                logger.LogInformation($"Temperature is greater than 75, turning both fans on.");
                relayService.TurnAllOn();
            }
        }
    }
}
