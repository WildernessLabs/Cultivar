using Cultivar.Commands;
using Cultivar.MeadowApp.Controllers;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Relays;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp
{
    public class GreenhouseController
    {
        // Singleton pattern (in case we want to change)
        //private static readonly Lazy<GreenhouseController> lazy = new Lazy<GreenhouseController>(() => new GreenhouseController());
        //public static GreenhouseController Current { get { return lazy.Value; } }
        //private GreenhouseController() { }
        //public Initialize(IGreenhouseHardware greenhouseHardware)

        protected IGreenhouseHardware Hardware { get; set; }

        protected DisplayController displayController;
        protected MicroAudio audio;
        protected CloudLogger cloudLogger;

        protected TimeSpan UpdateInterval = TimeSpan.FromSeconds(60);

        public GreenhouseController(IGreenhouseHardware greenhouseHardware)
        {
            Hardware = greenhouseHardware;

            cloudLogger = new CloudLogger(LogLevel.Warning);
            Resolver.Log.AddProvider(cloudLogger);
            Resolver.Services.Add(cloudLogger);

            Resolver.Log.Info($"cloudlogger null? {cloudLogger is null}");

            Hardware.RgbLed?.SetColor(Color.Red);

            if (Hardware.Display is { } display)
            {
                Resolver.Log.Trace("Creating DisplayController");
                displayController = new DisplayController(display);
                Resolver.Log.Trace("DisplayController up");
            }

            Hardware.Speaker?.SetVolume(0.5f);
            audio = new MicroAudio(Hardware.Speaker);

            if (Hardware.EnvironmentalSensor is { } bme688) { bme688.Updated += Bme688Updated; }

            if (Hardware.MoistureSensor is { } moisture) { moisture.Updated += MoistureUpdated; }

            WireUpButtons();
            SubscribeToCommands();
            //HandleRelayChanges();

            //---- cloud status
            //displayController.CloudConnectionStatus = Resolver.UpdateService.State.ToString();
            SubscribeToCloudConnectionEvents();

            // wifi events
            var wifi = Resolver.Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (wifi.IsConnected)
            {
                displayController.UpdateWifi(true);
            }
            wifi.NetworkConnected += (networkAdapter, networkConnectionEventArgs) =>
            {
                Resolver.Log.Info("Joined network");
                Console.WriteLine($"IP Address: {networkAdapter.IpAddress}.");
                displayController.UpdateWifi(true);
                _ = audio.PlaySystemSound(SystemSoundEffect.Chime);
            };
            wifi.NetworkDisconnected += sender =>
            {
                displayController.UpdateWifi(false);
            };

            Resolver.Log.Info("Initialization complete");
        }

        void SubscribeToCloudConnectionEvents()
        {
            Resolver.UpdateService.OnStateChanged += (sender, state) =>
            {
                if (state.ToString() == "Connected")
                {
                    displayController.UpdateSync(true);
                }

                //displayController.UpdateStatus(state.ToString());
            };
        }

        void SubscribeToCommands()
        {
            Resolver.CommandService.Subscribe<Fan>(c =>
            {
                Resolver.Log.Info($"Received fan control: {c.IsOn}");
                displayController.UpdateVents(c.IsOn);
                Hardware.VentFan.IsOn = c.IsOn;
            });

            Resolver.CommandService.Subscribe<Heater>(c =>
            {
                Resolver.Log.Info($"Received heater control: {c.IsOn}");
                displayController.UpdateHeater(c.IsOn);
                Hardware.Heater.IsOn = c.IsOn;
            });

            Resolver.CommandService.Subscribe<Lights>(c =>
            {
                Resolver.Log.Info($"Received light control: {c.IsOn}");
                displayController.UpdateLights(c.IsOn);
                Hardware.Lights.IsOn = c.IsOn;
            });

            Resolver.CommandService.Subscribe<Irrigation>(c =>
            {
                Resolver.Log.Info($"Received valve control: {c.IsOn}");
                displayController.UpdateWater(c.IsOn);
                Hardware.IrrigationLines.IsOn = c.IsOn;
            });

            //Resolver.CommandService.Subscribe(c =>
            //{
            //    Resolver.Log.Info($"Received command: {c.CommandName} with args {c.Arguments}");
            //});

            //Resolver.CommandService.Subscribe<FanControl>(e =>
            //{
            //    Resolver.Log.Trace($"Received fan control: {e.RelayState}");
            //});
        }

        void WireUpButtons()
        {
            //if (Hardware.RightButton is { } rightButton)
            //{
            //    rightButton.PressStarted += (s, e) =>
            //    {
            //        displayController.HeaterState = true;
            //        //Hardware.Heater.IsOn = true;

            //        Resolver.Log.Trace($"relay changed, IsHeaterOn:true");
            //        try
            //        {
            //            var cl = Resolver.Services.Get<CloudLogger>();
            //            cl.LogEvent(110, "relay change", new Dictionary<string, object>()
            //            {
            //                { "IsHeaterOn", true }
            //            });
            //        }
            //        catch (Exception ex)
            //        {
            //            Resolver.Log.Info($"Err: {ex.Message}");
            //        }

            //    };
            //    rightButton.PressEnded += (s, e) =>
            //    {
            //        displayController.HeaterState = false;
            //        //Hardware.Heater.IsOn = false;

            //        Resolver.Log.Trace($"relay changed, IsHeaterOn:false");
            //        try
            //        {
            //            var cl = Resolver.Services.Get<CloudLogger>();
            //            cl.LogEvent(110, "relay change", new Dictionary<string, object>()
            //            {
            //                { "IsHeaterOn", false }
            //            });
            //        }
            //        catch (Exception ex)
            //        {
            //            Resolver.Log.Info($"Err: {ex.Message}");
            //        }
            //    };
            //}

            if (Hardware.DownButton is { } downButton)
            {
                Resolver.Log.Info($"DownButton");

                downButton.PressStarted += (s, e) =>
                {
                    Resolver.Log.Info($"downButton.PressStarted");
                    displayController.UpdateWater(true);
                    Hardware.IrrigationLines.IsOn = true;
                };
                downButton.PressEnded += (s, e) =>
                {
                    Resolver.Log.Info($"downButton.PressEnded");
                    displayController.UpdateWater(false);
                    Hardware.IrrigationLines.IsOn = false;
                };
            }
            if (Hardware.LeftButton is { } leftButton)
            {
                Resolver.Log.Info($"LeftButton");

                leftButton.PressStarted += (s, e) =>
                {
                    Resolver.Log.Info($"leftButton.PressStarted");
                    displayController.UpdateLights(true);
                    Hardware.Lights.IsOn = true;
                };
                leftButton.PressEnded += (s, e) =>
                {
                    Resolver.Log.Info($"leftButton.PressEnded");
                    displayController.UpdateLights(false);
                    Hardware.Lights.IsOn = false;
                };
            }
            if (Hardware.UpButton is { } upButton)
            {
                Resolver.Log.Info($"UpButton");

                upButton.PressStarted += (s, e) =>
                {
                    Resolver.Log.Info($"upButton.PressStarted");
                    displayController.UpdateVents(true);
                    Hardware.VentFan.IsOn = true;
                };
                upButton.PressEnded += (s, e) =>
                {
                    Resolver.Log.Info($"upButton.PressEnded");
                    displayController.UpdateVents(false);
                    Hardware.VentFan.IsOn = false;
                };
            }
        }

        //==== HANDLERS
        //TODO: would be nice to wait until a full set of readings has been made and send together.
        // but that woudl be pretty difficult to synchronize over time

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688: {(int)e.New.Temperature?.Celsius}C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");

            if (displayController != null)
            {
                displayController.UpdateTemperature(e.New.Temperature.Value.Celsius);
                displayController.UpdateHumidity(e.New.Humidity.Value.Percent);
                displayController.UpdateSoilMoisture(e.New.Humidity.Value.Percent - 10);
            }

            Resolver.Log.Info($"Logging BME688 reading to cloud.");

            try
            {
                var cl = Resolver.Services.Get<CloudLogger>();
                cl.LogEvent(110, "Atmospheric reading", new Dictionary<string, object>()
                {
                    { "TemperatureCelsius", e.New.Temperature?.Celsius },
                    { "HumidityPercent", e.New.Humidity?.Percent },
                    { "PressureMillibar", e.New.Pressure?.Millibar }
                });
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"Err: {ex.Message}");
            }
        }

        private void MoistureUpdated(object sender, IChangeResult<double> result)
        {
            string oldValue = result.Old is { } old ? $"{old:n2}" : "n/a"; // C# 8 pattern matching

            Resolver.Log.Info($"Moisture Updated - New: {result.New}, Old: {oldValue}");

            //displayController.UpdateHumidity(result.New);

            Resolver.Log.Info($"Logging Moisture reading to cloud.");
            try
            {
                var cl = Resolver.Services.Get<CloudLogger>();
                cl.LogEvent(110, "Moisture reading", new Dictionary<string, object>()
                {
                    { "MoisturePercent", result.New },
                });
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"Err: {ex.Message}");
            }
        }

        private void HandleRelayChanges()
        {
            RegisterRelayChange(Hardware.VentFan, "IsVentilationOn");
            RegisterRelayChange(Hardware.Heater, "IsHeaterOn");
            RegisterRelayChange(Hardware.Lights, "IsLightOn");
            RegisterRelayChange(Hardware.IrrigationLines, "IsIrrigationOn");
        }

        private void RegisterRelayChange(IRelay relay, string eventName)
        {
            relay.OnRelayChanged += (sender, relayState) =>
            {
                Resolver.Log.Trace($"relay changed, {eventName}:{relayState}");
                try
                {
                    var cl = Resolver.Services.Get<CloudLogger>();
                    cl.LogEvent(110, "relay change", new Dictionary<string, object>()
                    {
                        { eventName, relayState }
                    });
                }
                catch (Exception ex)
                {
                    Resolver.Log.Info($"Err: {ex.Message}");
                }
            };
        }

        public Task Run()
        {
            _ = audio.PlaySystemSound(SystemSoundEffect.Fanfare);

            if (Hardware.EnvironmentalSensor is { } bme688) { bme688.StartUpdating(UpdateInterval); }

            if (Hardware.MoistureSensor is { } moisture) { moisture.StartUpdating(UpdateInterval); }

            Resolver.Log.Info("starting blink");

            _ = Hardware.RgbLed?.StartBlink(WildernessLabsColors.PearGreen, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(2000), 0.5f);

            return Task.CompletedTask;
        }
    }
}