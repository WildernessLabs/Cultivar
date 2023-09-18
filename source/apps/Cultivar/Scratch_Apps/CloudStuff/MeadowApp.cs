using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Leds;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meadow.Logging;
using MeadowApp.Commands;
using RelativeHumidity = Meadow.Units.RelativeHumidity;
using Resistance = Meadow.Units.Resistance;

namespace MeadowApp
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private DisplayController displayController;
        private IProjectLabHardware projLab;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");
            
            projLab = ProjectLab.Create();
            
            var cloudLogger = new CloudLogger();
            Resolver.Log.AddProvider(cloudLogger);
            Resolver.Services.Add(cloudLogger);
            
            if (projLab.Display is { } display)
            {
                Resolver.Log.Trace("Creating DisplayController");
                displayController = new DisplayController(display);
                Resolver.Log.Trace("DisplayController up");
            }
            
            if (projLab.EnvironmentalSensor is { } bme688)
            {
                bme688.Updated += Bme688Updated;
                bme688.StartUpdating(TimeSpan.FromMinutes(1));
            }
            
            return base.Initialize();
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

        private void Bme688Updated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Trace($"BME688: {(int)e.New.Temperature?.Celsius}C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");
            if (displayController != null)
            {
                displayController.AtmosphericConditions = e.New;
            }
            
            var cl = Resolver.Services.Get<CloudLogger>();
            cl.LogEvent(110, "Atmospheric reading", new Dictionary<string, object>()
            {
                { "TemperatureCelsius", e.New.Temperature?.Celsius },
                { "HumidityPercentage", e.New.Humidity?.Percent },
                { "PressureMillibar", e.New.Pressure?.Millibar }
            });
        }
    }
}