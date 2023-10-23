using Cultivar.Controllers;
using Cultivar.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        GreenhouseController greenhouseController;
        int WatchdogUptimeMaxHours = 1;
        int WatchdogUptimePetCountMax = 0;
        int WatchdogCount = 0;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize hardware...");

            var greenhouseHardware = new ProductionBetaHardware();
            greenhouseController = new GreenhouseController(greenhouseHardware);

            WireUpWiFiStatusEvents();

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

        void WireUpWiFiStatusEvents()
        {
            // get the wifi adapter
            var wifi = Resolver.Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            // set initial state
            if (wifi.IsConnected) {
                greenhouseController?.SetWiFiStatus(true);
                Resolver.Log.Info("Already connected to WiFi.");
            }
            else {
                greenhouseController?.SetWiFiStatus(false);
                Resolver.Log.Info("Not connected to WiFi yet.");
            }
            // connect event
            wifi.NetworkConnected += (networkAdapter, networkConnectionEventArgs) =>
            {
                Resolver.Log.Info($"Joined network - IP Address: {networkAdapter.IpAddress}");
                greenhouseController?.SetWiFiStatus(true);
                //_ = audio?.PlaySystemSound(SystemSoundEffect.Chime);
            };
            // disconnect event
            wifi.NetworkDisconnected += sender => { greenhouseController?.SetWiFiStatus(false); };
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