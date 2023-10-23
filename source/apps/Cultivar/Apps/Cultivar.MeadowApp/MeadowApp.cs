using Cultivar.Controllers;
using Cultivar.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        GreenhouseController greenhouseController;
        int WatchdogUptimeMaxHours = 3;
        int WatchdogUptimePetCountMax = 0;
        int WatchdogCount = 0;

        public override Task Initialize()
        {
            //var cloudLogger = new CloudLogger(LogLevel.Warning);
            //Resolver.Log.AddProvider(cloudLogger);
            //Resolver.Services.Add(cloudLogger);

            //Resolver.Log.Info($"cloudlogger null? {cloudLogger is null}");

            Resolver.Log.Info("Initialize hardware...");

            var greenhouseHardware = new ProductionBetaHardware();
            greenhouseController = new GreenhouseController(greenhouseHardware);

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            // Enable the watchdog for 30 second intervals (max is ~32s)
            Device.WatchdogEnable(TimeSpan.FromSeconds(30));
            // calculate the number of times we need to pet the watchdog.
            WatchdogUptimePetCountMax = ((WatchdogUptimeMaxHours * 60 * 60) / 30);
            // Start the thread that resets the counter.
            StartPettingWatchdog(TimeSpan.FromSeconds(9));

            greenhouseController.Run();

            return base.Run();
        }

        void StartPettingWatchdog(TimeSpan pettingInterval)
        {
            // Just for good measure, let's reset the watchdog to begin with.
            Device.WatchdogReset();
            // Start a thread that restarts it.
            Thread t = new Thread(async () => {
                while (true)
                {
                    if (WatchdogCount <= WatchdogUptimePetCountMax) {
                        Thread.Sleep(pettingInterval);
                        Console.WriteLine("Petting watchdog.");
                        Device.WatchdogReset();
                    } else
                    {
                        // stop spinning while it restarts
                        await Task.Delay(1000);
                    }
                    WatchdogCount++;
                }
            });
            t.Start();
        }
    }
}