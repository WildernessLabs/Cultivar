using Meadow;
using Meadow.Devices;
using Meadow.Logging;
using MeadowApp.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeadowApp
{
    // Change ProjectLabCoreComputeApp to ProjectLabFeatherApp for V1.x boards
    public class MeadowApp : ProjectLabCoreComputeApp
    {
        private DisplayController displayController;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var cloudLogger = new CloudLogger();
            Resolver.Log.AddProvider(cloudLogger);
            Resolver.Services.Add(cloudLogger);

            if (Hardware.Display is { } display)
            {
                Resolver.Log.Trace("Creating DisplayController");
                displayController = new DisplayController(display);
                Resolver.Log.Trace("DisplayController up");
            }

            if (Hardware.TemperatureSensor is { } temperature)
            {
                temperature.Updated += TemperatureUpdated; ;
                temperature.StartUpdating(TimeSpan.FromMinutes(1));
            }

            return base.Initialize();
        }

        private void TemperatureUpdated(object sender, IChangeResult<Meadow.Units.Temperature> e)
        {
            Resolver.Log.Trace($"Temperature: {(int)e.New.Celsius}C");
            if (displayController != null)
            {
                displayController.TemperatureConditions = e.New;
            }

            var cl = Resolver.Services.Get<CloudLogger>();
            cl.LogEvent(110, "Atmospheric reading", new Dictionary<string, object>()
            {
                { "TemperatureCelsius", e.New.Celsius }
            });
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            displayController?.Update();

            Resolver.CommandService.Subscribe<FanControl>(e =>
            {
                Resolver.Log.Trace($"Received fan control: {e.RelayState}");
            });

            Resolver.CommandService.Subscribe<HeaterControl>(e =>
            {
                Resolver.Log.Trace($"Received heater control: {e.RelayState}");
            });

            Resolver.CommandService.Subscribe<LightControl>(e =>
            {
                Resolver.Log.Trace($"Received light control: {e.RelayState}");
            });

            Resolver.CommandService.Subscribe<ValveControl>(e =>
            {
                Resolver.Log.Trace($"Received valve control: {e.RelayState}");
            });

            return base.Run();
        }
    }
}