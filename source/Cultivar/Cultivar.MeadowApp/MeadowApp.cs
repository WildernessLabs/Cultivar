using Cultivar.Controllers;
using Cultivar.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using Meadow.Logging;
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

        var reliabilityService = Resolver.Services.Get<IReliabilityService>();
        reliabilityService.MeadowSystemError += OnMeadowSystemError;

        if (reliabilityService.LastBootWasFromCrash)
        {
            Resolver.Log.Info("Booting after a crash!");

            Resolver.Log.Info("Crash report:");
            foreach (var r in reliabilityService.GetCrashData())
            {
                Resolver.Log.Info(r);
            }

            Resolver.Log.Info("Clearing crash data...");
            reliabilityService.ClearCrashData();
        }

        Resolver.MeadowCloudService.SendLog(LogLevel.Information, "Cultivar started");
        Resolver.MeadowCloudService.ErrorOccurred += MeadowCloudService_ErrorOccurred;

        var greenhouseHardware = new ProductionBetaHardware();
        var networkAdapter = Device.NetworkAdapters.Primary<INetworkAdapter>();

        mainController = new MainController(greenhouseHardware, networkAdapter!);

        return base.Initialize();
    }

    private void OnMeadowSystemError(MeadowSystemErrorInfo error, bool recommendReset, out bool forceReset)
    {
        if (error is Esp32SystemErrorInfo espError)
        {
            Resolver.Log.Warn($"The ESP32 has had an error ({espError.StatusCode}).");
        }
        else
        {
            Resolver.Log.Info($"We've had a system error: {error}");
        }

        if (recommendReset)
        {
            Resolver.Log.Warn($"Meadow is recommending a device reset");
        }

        forceReset = recommendReset;

        // override the reset recommendation
        //forceReset = false;
    }

    private void MeadowCloudService_ErrorOccurred(object sender, Exception e)
    {
        Resolver.Log.Error($"CLOUD ERROR: {e.Message}");
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        WireUpWatchdogs();

        mainController.Run();

        return base.Run();
    }

    private void WireUpWatchdogs()
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

    private void StartPettingWatchdog(TimeSpan pettingInterval)
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