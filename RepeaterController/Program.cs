/*
 * AUTHOR: David B. Dickey (KN4TEM) 
 * Written 8/31/2022 for CARC special project.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using HidLibrary;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using UsbRelayTest.Extensions;

namespace UsbRelayTest
{
    public class Program
    {

        const int tempCheckInterval = 30000;        //TODO: make configurable at run time

        public static async Task Main(string[] args)
        {
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

            using (Timer tempTimer = new Timer(interval: tempCheckInterval))
            {
                Random rand = new Random(23984256);
                I2CThermometer thermometer = new I2CThermometer(serilogFactory.CreateLogger<I2CThermometer>());
                RelayService relayService = new RelayService(serilogFactory.CreateLogger<RelayService>());
                tempTimer.Elapsed += (sender, e) => TemperatureCheckHandler(logger, rand, thermometer, relayService);
                tempTimer.Start();
            }
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
            RelayService relayService)
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
