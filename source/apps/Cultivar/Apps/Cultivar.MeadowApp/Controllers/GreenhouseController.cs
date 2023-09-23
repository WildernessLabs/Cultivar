using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cultivar.MeadowApp.UI;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Gateways.Bluetooth;
using Meadow.Logging;
using Meadow.Units;
using Meadow.Foundation.Sensors.Moisture;
using Meadow.Devices;
using Cultivar.Commands;
using Meadow.Hardware;
using Meadow.Update;

namespace Cultivar.MeadowApp.Controllers
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
        CloudLogger cloudLogger;

        protected TimeSpan UpdateInterval = TimeSpan.FromSeconds(60);

        public GreenhouseController(IGreenhouseHardware greenhouseHardware)
        {
            this.Hardware = greenhouseHardware;

            cloudLogger = new CloudLogger(LogLevel.Warning);
            Resolver.Log.AddProvider(cloudLogger);
            Resolver.Services.Add(cloudLogger);

            Resolver.Log.Info($"cloudlogger null? {cloudLogger is null}");

            Hardware.RgbLed?.SetColor(Color.Blue);

            Hardware.Speaker?.SetVolume(0.5f);
            audio = new MicroAudio(Hardware.Speaker);

            //---- display controller (handles display updates)
            if (Hardware.Display is { } display)
            {
                Resolver.Log.Trace("Creating DisplayController");
                displayController = new DisplayController(display);
                Resolver.Log.Trace("DisplayController up");
            }

            //---- BH1750 Light Sensor
            if (Hardware.LightSensor is { } bh1750) { bh1750.Updated += Bh1750Updated; }

            //---- BME688 Atmospheric sensor
            if (Hardware.EnvironmentalSensor is { } bme688) { bme688.Updated += Bme688Updated; }

            //---- BMI270 Accel/IMU
            if (Hardware.MotionSensor is { } bmi270) { bmi270.Updated += Bmi270Updated; }

            //---- moisture sensor
            if (Hardware.MoistureSensor is { } moisture) { moisture.Updated += MoistureUpdated; }

            //---- buttons
            this.WireUpButtons();

            //---- commands
            this.SubscribeToCommands();
            
            //---- cloud status
            displayController.CloudConnectionStatus = Resolver.UpdateService.State.ToString();
            this.SubscribeToCloudConnectionEvents();

            // wifi events
            var wifi = Resolver.Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (wifi.IsConnected)
            {
                displayController.WiFiConnected = true;
            }
            wifi.NetworkConnected += (networkAdapter, networkConnectionEventArgs) =>
            {
                Resolver.Log.Info("Joined network");
                Console.WriteLine($"IP Address: {networkAdapter.IpAddress}.");
                displayController.WiFiConnected = true;
                _ = audio.PlaySystemSound(SystemSoundEffect.Chime);
            };
            wifi.NetworkDisconnected += sender =>
            {
                displayController.WiFiConnected = false;
            };
            
            //Resolver.UpdateService.State

            //---- heartbeat
            Resolver.Log.Info("Initialization complete");
        }

        public Task Run()
        {
            _ = audio.PlaySystemSound(SystemSoundEffect.Fanfare);

            //---- BH1750 Light Sensor
            if (Hardware.LightSensor is { } bh1750) { bh1750.StartUpdating(UpdateInterval); }

            //---- BME688 Atmospheric sensor
            if (Hardware.EnvironmentalSensor is { } bme688) { bme688.StartUpdating(UpdateInterval); }

            //---- BMI270 Accel/IMU
            if (Hardware.MotionSensor is { } bmi270) { bmi270.StartUpdating(UpdateInterval); }

            //---- moisture
            if (Hardware.MoistureSensor is { } moisture) { moisture.StartUpdating(UpdateInterval); }

            displayController?.Update();

            Resolver.Log.Info("starting blink");
            _ = Hardware.RgbLed?.StartBlink(WildernessLabsColors.PearGreen, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(2000), 0.5f);


            return Task.CompletedTask;
        }

        void SubscribeToCloudConnectionEvents()
        {
            Resolver.UpdateService.OnStateChanged += (sender, state) =>
            {
                displayController.CloudConnectionStatus = state.ToString();
            };
        }

        void SubscribeToCommands()
        {
            Resolver.CommandService.Subscribe<Fan>(c =>
            {
                Resolver.Log.Info($"Received fan control: {c.IsOn}");
                displayController.FanState = c.IsOn;
                Hardware.VentFan.IsOn = c.IsOn;
            });

            Resolver.CommandService.Subscribe<Heater>(c =>
            {
                Resolver.Log.Info($"Received heater control: {c.IsOn}");
                displayController.HeaterState = c.IsOn;
                Hardware.Heater.IsOn = c.IsOn;
            });

            Resolver.CommandService.Subscribe<Lights>(c =>
            {
                Resolver.Log.Info($"Received light control: {c.IsOn}");
                displayController.LightsState = c.IsOn;
                Hardware.Lights.IsOn = c.IsOn;
            });

            Resolver.CommandService.Subscribe<Irrigation>(c =>
            {
                Resolver.Log.Info($"Received valve control: {c.IsOn}");
                displayController.IrrigationState = c.IsOn;
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
            if (Hardware.RightButton is { } rightButton)
            {
                rightButton.PressStarted += (s, e) =>
                {
                    displayController.HeaterState = true;
                    Hardware.Heater.IsOn = true;
                };
                rightButton.PressEnded += (s, e) =>
                {
                    displayController.HeaterState = false;
                    Hardware.Heater.IsOn = false;
                };
            }

            if (Hardware.DownButton is { } downButton)
            {
                downButton.PressStarted += (s, e) =>
                {
                    displayController.IrrigationState = true;
                    Hardware.IrrigationLines.IsOn = true;
                };
                downButton.PressEnded += (s, e) =>
                {
                    displayController.IrrigationState = false;
                    Hardware.IrrigationLines.IsOn = false;
                };
            }
            if (Hardware.LeftButton is { } leftButton)
            {
                leftButton.PressStarted += (s, e) =>
                {
                    displayController.LightsState = true;
                    Hardware.Lights.IsOn = true;
                };
                leftButton.PressEnded += (s, e) =>
                {
                    displayController.LightsState = false;
                    Hardware.Lights.IsOn = false;
                };
            }
            if (Hardware.UpButton is { } upButton)
            {
                upButton.PressStarted += (s, e) =>
                {
                    displayController.FanState = true;
                    Hardware.VentFan.IsOn = true;
                };
                upButton.PressEnded += (s, e) =>
                {
                    displayController.FanState = false;
                    Hardware.VentFan.IsOn = false;
                };
            }
        }

        //==== HANDLERS
        //TODO: would be nice to wait until a full set of readings has been made and send together.
        // but that woudl be pretty difficult to synchronize over time

        private void Bmi270Updated(object sender, IChangeResult<(Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature)> e)
        {
            Resolver.Log.Info($"BMI270: {e.New.Acceleration3D.Value.X.Gravity:0.0},{e.New.Acceleration3D.Value.Y.Gravity:0.0},{e.New.Acceleration3D.Value.Z.Gravity:0.0}g");
            if (displayController != null)
            {
                displayController.AccelerationConditions = e.New;
            }
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688: {(int)e.New.Temperature?.Celsius}C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");
            if (displayController != null)
            {
                displayController.AtmosphericConditions = e.New;
            }
            Resolver.Log.Info($"Logging BME688 reading to cloud.");
            try
            {
                var cl = Resolver.Services.Get<CloudLogger>();
                cl.LogEvent(110, "Atmospheric reading", new Dictionary<string, object>()
                {
                    { "TemperatureCelsius", e.New.Temperature?.Celsius },
                    { "HumidityPercentage", e.New.Humidity?.Percent },
                    { "PressureMillibar", e.New.Pressure?.Millibar }
                });
            }
            catch (Exception ex) {
                Resolver.Log.Info($"Err: {ex.Message}");
            }
        }

        private void MoistureUpdated(object sender, IChangeResult<double> result)
        {
            string oldValue = (result.Old is { } old) ? $"{old:n2}" : "n/a"; // C# 8 pattern matching
            Resolver.Log.Info($"Moisture Updated - New: {result.New}, Old: {oldValue}");
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

        private void Bh1750Updated(object sender, IChangeResult<Illuminance> e)
        {
            Resolver.Log.Info($"BH1750: {e.New.Lux}");
            if (displayController != null)
            {
                displayController.LightConditions = e.New;
            }
            Resolver.Log.Info($"Logging Light sensor reading to cloud.");
            try
            {
                var cl = Resolver.Services.Get<CloudLogger>();
                cl.LogEvent(110, "Light Sensor", new Dictionary<string, object>()
                {
                    { "LightLux", e.New.Lux },
                });
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"Err: {ex.Message}");
            }
        }

    }
}