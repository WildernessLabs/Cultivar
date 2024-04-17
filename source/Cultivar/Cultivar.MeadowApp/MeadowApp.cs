using Cultivar.Controllers;
using Cultivar.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp;

public class MeadowApp : App<F7CoreComputeV2>
{
    private MainController mainController;
    //private int WatchdogUptimeMaxHours = 1;
    //private int WatchdogUptimePetCountMax = 0;
    //private int WatchdogCount = 0;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize hardware...");

        var greenhouseHardware = new ProductionBetaHardware();
        var networkAdapter = Device.NetworkAdapters.Primary<INetworkAdapter>();

        mainController = new MainController(greenhouseHardware, networkAdapter!);

        return base.Initialize();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        WireUpWatchdogs();

        mainController.Run();

        return base.Run();
    }

    void WireUpWatchdogs()
    {
        var watchdogTimeout = TimeSpan.FromSeconds(30);
        var pettingInterval = TimeSpan.FromSeconds(20); // should be well less than the timeout

        // Enable the watchdog for 30 second intervals (max is ~32s)
        Device.WatchdogEnable(watchdogTimeout);
        // calculate the number of times we need to pet the watchdog.
        //WatchdogUptimePetCountMax = ((WatchdogUptimeMaxHours * 60 * 60) / 30);
        // Start the thread that resets the counter.
        StartPettingWatchdog(pettingInterval);
    }

    void StartPettingWatchdog(TimeSpan pettingInterval)
    {
        // Just for good measure, let's reset the watchdog to begin with.
        Device.WatchdogReset();
        // Start a thread that restarts it.
        Thread t = new Thread(async () =>
        {
            while (true)
            {
                // if (WatchdogCount <= WatchdogUptimePetCountMax)
                // {
                Thread.Sleep(pettingInterval);
                Device.WatchdogReset();
                //}
                // else
                // {
                //     Resolver.Log.Warn("Max uptime has elapsed. Restarting to maintain stability.");
                //     // stop spinning while the watchdog countdown elapses
                //     Thread.Sleep(pettingInterval * 2);
                // }
                //WatchdogCount++;
            }
        });
        t.Start();
    }
}