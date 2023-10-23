using Cultivar.Commands;
using Cultivar.MeadowApp.Controllers;
using Cultivar.MeadowApp.Models;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultivar.Controllers
{
    public class GreenhouseController
    {
        protected bool IsSampling = false;

        private bool isVentilationOn = false;
        private bool isIrrigationOn = false;
        private bool isLightOn = false;
        private bool isHeaterOn = false;

        protected IGreenhouseHardware Hardware { get; set; }

        protected DisplayController displayController;
        //protected MicroAudio audio;
        protected CloudLogger cloudLogger;

        protected TimeSpan UpdateInterval = TimeSpan.FromSeconds(60);

        GreenhouseModel Climate;

        public GreenhouseController(IGreenhouseHardware greenhouseHardware, bool isSimulator = false)
        {
            Hardware = greenhouseHardware;

            cloudLogger = new CloudLogger(LogLevel.Warning);
            Resolver.Log.AddProvider(cloudLogger);
            Resolver.Services.Add(cloudLogger);

            Resolver.Log.Info($"cloudlogger null? {cloudLogger is null}");

            Hardware.RgbLed?.SetColor(Color.Red);

            if (Hardware.Display is { } display)
            {
                displayController = new DisplayController(display, isSimulator ? RotationType.Normal : RotationType._270Degrees);
            }

            //if (Hardware.Speaker is { } speaker)
            //{
            //    speaker.SetVolume(0.5f);
            //    audio = new MicroAudio(speaker);
            //}

            SubscribeToCloudConnectionEvents();

            if (!isSimulator)
            {
                SubscribeToCommands();

                //HandleRelayChanges();
            }

            InitializeButtons();

            Hardware.RgbLed?.SetColor(Color.Green);
            Resolver.Log.Info("Initialization complete");
        }

        private void InitializeButtons()
        {
            if (Hardware.UpButton is { } ventilationButton)
            {
                ventilationButton.Clicked += (s, e) =>
                {
                    isVentilationOn = !isVentilationOn;

                    displayController.UpdateVents(isVentilationOn);

                    if (Hardware.VentFan is { } ventilation)
                    {
                        ventilation.IsOn = isVentilationOn;
                    }

                    try
                    {
                        var cl = Resolver.Services.Get<CloudLogger>();
                        cl?.LogEvent(110, "Ventilation relay change", new Dictionary<string, object>()
                        {
                            { "IsVentilationOn", isVentilationOn }
                        });
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Info($"Err: {ex.Message}");
                    }
                };
            }
            if (Hardware.DownButton is { } irrigationButton)
            {
                irrigationButton.Clicked += (s, e) =>
                {
                    isIrrigationOn = !isIrrigationOn;

                    displayController.UpdateWater(isIrrigationOn);

                    if (Hardware.IrrigationLines is { } irrigation)
                    {
                        irrigation.IsOn = isIrrigationOn;
                    }

                    try
                    {
                        var cl = Resolver.Services.Get<CloudLogger>();
                        cl?.LogEvent(110, "Irrigation relay change", new Dictionary<string, object>()
                        {
                            { "IsIrrigationOn", isIrrigationOn }
                        });
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Info($"Err: {ex.Message}");
                    }
                };
            }
            if (Hardware.LeftButton is { } lightButton)
            {
                lightButton.Clicked += (s, e) =>
                {
                    isLightOn = !isLightOn;

                    displayController.UpdateLights(isLightOn);

                    if (Hardware.Lights is { } lights)
                    {
                        lights.IsOn = isLightOn;
                    }

                    try
                    {
                        var cl = Resolver.Services.Get<CloudLogger>();
                        cl?.LogEvent(110, "Light relay change", new Dictionary<string, object>()
                        {
                            { "IsLightOn", isLightOn }
                        });
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Info($"Err: {ex.Message}");
                    }
                };
            }
            if (Hardware.RightButton is { } heaterButton)
            {
                heaterButton.Clicked += (s, e) =>
                {
                    isHeaterOn = !isHeaterOn;

                    displayController.UpdateHeater(isHeaterOn);

                    if (Hardware.Heater is { } heater)
                    {
                        heater.IsOn = isHeaterOn;
                    }

                    try
                    {
                        var cl = Resolver.Services.Get<CloudLogger>();
                        cl?.LogEvent(110, "Heater relay change", new Dictionary<string, object>()
                        {
                            { "IsHeaterOn", isHeaterOn }
                        });
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Info($"Err: {ex.Message}");
                    }
                };
            }
        }

        public void SetWiFiStatus(bool connected)
        {
            displayController.UpdateWifi(connected);
        }

        private async Task StartUpdating(TimeSpan updateInterval)
        {
            Console.WriteLine("ClimateMonitorAgent.StartUpdating()");

            if (IsSampling)
                return;
            IsSampling = true;

            while (IsSampling)
            {
                Console.WriteLine("ClimateMonitorAgent: About to do a reading.");

                Climate = await Read();

                Console.WriteLine($"Temperature: {Climate.Temperature.Celsius} | Humidity: {Climate.Humidity.Percent} | Moisture: {Climate.SoilMoisture}");

                displayController.UpdateReadings(Climate.Temperature.Celsius, Climate.Humidity.Percent, Climate.SoilMoisture);

                try
                {
                    displayController.UpdateSync(true);
                    var cl = Resolver.Services.Get<CloudLogger>();
                    cl?.LogEvent(110, "Atmospheric reading", new Dictionary<string, object>()
                    {
                        { "TemperatureCelsius", Climate.Temperature.Celsius },
                        { "HumidityPercent", Climate.Humidity.Percent },
                        { "PressureMillibar", Climate.SoilMoisture }
                    });
                    displayController.UpdateSync(false);
                }
                catch (Exception ex)
                {
                    Resolver.Log.Info($"Err: {ex.Message}");
                }

                await Task.Delay(updateInterval);
            }
        }

        private void StopUpdating()
        {
            if (!IsSampling)
                return;

            IsSampling = false;
        }

        private async Task<GreenhouseModel> Read()
        {
            var temperatureTask = Hardware.TemperatureSensor?.Read();
            var humidityTask = Hardware.HumiditySensor?.Read();
            var moistureTask = Hardware.MoistureSensor?.Read();

            await Task.WhenAll(temperatureTask, humidityTask, moistureTask);

            var climate = new GreenhouseModel()
            {
                Temperature = temperatureTask?.Result ?? new Meadow.Units.Temperature(0),
                Humidity = humidityTask?.Result ?? new Meadow.Units.RelativeHumidity(0),
                SoilMoisture = moistureTask?.Result ?? 0
            };

            return climate;
        }

        private void SubscribeToCommands()
        {
            Resolver.CommandService?.Subscribe<Fan>(c =>
            {
                Resolver.Log.Info($"Received fan control: {c.IsOn}");
                displayController.UpdateWater(c.IsOn);
                if (Hardware.VentFan != null)
                {
                    Hardware.VentFan.IsOn = c.IsOn;
                }
            });
            Resolver.CommandService?.Subscribe<Heater>(c =>
            {
                Resolver.Log.Info($"Received heater control: {c.IsOn}");
                displayController.UpdateHeater(c.IsOn);
                if (Hardware.Heater != null)
                {
                    Hardware.Heater.IsOn = c.IsOn;
                }
            });
            Resolver.CommandService?.Subscribe<Lights>(c =>
            {
                Resolver.Log.Info($"Received light control: {c.IsOn}");
                displayController.UpdateLights(c.IsOn);
                if (Hardware.Lights != null)
                {
                    Hardware.Lights.IsOn = c.IsOn;
                }
            });
            Resolver.CommandService?.Subscribe<Irrigation>(c =>
            {
                Resolver.Log.Info($"Received valve control: {c.IsOn}");
                displayController.UpdateVents(c.IsOn);
                if (Hardware.IrrigationLines != null)
                {
                    Hardware.IrrigationLines.IsOn = c.IsOn;
                }
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

        private void SubscribeToCloudConnectionEvents()
        {
            displayController?.UpdateStatus(Resolver.UpdateService.State.ToString());

            Resolver.UpdateService.OnStateChanged += (sender, state) =>
            {
                displayController?.UpdateStatus(state.ToString());
            };
        }

        //private void HandleRelayChanges()
        //{
        //    RegisterRelayChange(Hardware.VentFan, "IsVentilationOn");
        //    RegisterRelayChange(Hardware.Heater, "IsHeaterOn");
        //    RegisterRelayChange(Hardware.Lights, "IsLightOn");
        //    RegisterRelayChange(Hardware.IrrigationLines, "IsIrrigationOn");
        //}

        //private void RegisterRelayChange(IRelay relay, string eventName)
        //{
        //    relay.OnRelayChanged += (sender, relayState) =>
        //    {
        //        Resolver.Log.Trace($"relay changed, {eventName}:{relayState}");
        //        try
        //        {
        //            var cl = Resolver.Services.Get<CloudLogger>();
        //            cl?.LogEvent(110, "relay change", new Dictionary<string, object>()
        //            {
        //                { eventName, relayState }
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            Resolver.Log.Info($"Err: {ex.Message}");
        //        }
        //    };
        //}

        public Task Run()
        {
            //_ = audio.PlaySystemSound(SystemSoundEffect.Fanfare);

            _ = StartUpdating(TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }
    }
}