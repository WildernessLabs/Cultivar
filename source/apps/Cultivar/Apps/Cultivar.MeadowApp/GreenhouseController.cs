using Cultivar.Commands;
using Cultivar.MeadowApp.Controllers;
using Cultivar.MeadowApp.Models;
using Meadow;
using Meadow.Foundation;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp
{
    public class GreenhouseController
    {
        protected bool IsSampling = false;

        protected IGreenhouseHardware Hardware { get; set; }

        protected DisplayController displayController;
        //protected MicroAudio audio;
        protected CloudLogger cloudLogger;

        protected TimeSpan UpdateInterval = TimeSpan.FromSeconds(60);

        GreenhouseModel Climate;

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
                displayController = new DisplayController(display);
            }

            //if (Hardware.Speaker is { } speaker)
            //{
            //    speaker.SetVolume(0.5f);
            //    audio = new MicroAudio(speaker);
            //}

            InitializeButtons();

            SubscribeToCloudConnectionEvents();

            SubscribeToCommands();

            //HandleRelayChanges();

            InitializeWifi();

            Hardware.RgbLed?.SetColor(Color.Green);
            Resolver.Log.Info("Initialization complete");
        }

        private void InitializeButtons()
        {
            if (Hardware.UpButton is { } upButton)
            {
                Resolver.Log.Info($"UpButton");

                upButton.PressStarted += (s, e) =>
                {
                    Resolver.Log.Info($"upButton.PressStarted");
                    displayController.UpdateVents(true);
                    if (Hardware.VentFan is { } ventFan)
                    {
                        ventFan.IsOn = true;
                    }
                };
                upButton.PressEnded += (s, e) =>
                {
                    Resolver.Log.Info($"upButton.PressEnded");
                    displayController.UpdateVents(false);
                    if (Hardware.VentFan is { } ventFan)
                    {
                        ventFan.IsOn = false;
                    }
                };
            }
            if (Hardware.DownButton is { } downButton)
            {
                Resolver.Log.Info($"DownButton");

                downButton.PressStarted += (s, e) =>
                {
                    Resolver.Log.Info($"downButton.PressStarted");
                    displayController.UpdateWater(true);
                    if (Hardware.IrrigationLines is { } irrigationLines)
                    {
                        irrigationLines.IsOn = true;
                    }
                };
                downButton.PressEnded += (s, e) =>
                {
                    Resolver.Log.Info($"downButton.PressEnded");
                    displayController.UpdateWater(false);
                    if (Hardware.IrrigationLines is { } irrigationLines)
                    {
                        irrigationLines.IsOn = false;
                    }
                };
            }
            if (Hardware.LeftButton is { } leftButton)
            {
                Resolver.Log.Info($"LeftButton");

                leftButton.PressStarted += (s, e) =>
                {
                    Resolver.Log.Info($"leftButton.PressStarted");
                    displayController.UpdateLights(true);
                    if (Hardware.Lights is { } lights)
                    {
                        lights.IsOn = true;
                    }
                };
                leftButton.PressEnded += (s, e) =>
                {
                    Resolver.Log.Info($"leftButton.PressEnded");
                    displayController.UpdateLights(false);
                    if (Hardware.Lights is { } lights)
                    {
                        lights.IsOn = false;
                    }
                };
            }
            if (Hardware.RightButton is { } rightButton)
            {
                Resolver.Log.Info($"RightButton");

                rightButton.PressStarted += (s, e) =>
                {
                    Resolver.Log.Trace($"rightButton.PressStarted");
                    displayController.UpdateHeater(true);
                    if (Hardware.Heater is { } heater)
                    {
                        heater.IsOn = true;
                    }

                    try
                    {
                        var cl = Resolver.Services.Get<CloudLogger>();
                        cl.LogEvent(110, "relay change", new Dictionary<string, object>()
                        {
                            { "IsHeaterOn", true }
                        });
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Info($"Err: {ex.Message}");
                    }
                };
                rightButton.PressEnded += (s, e) =>
                {
                    Resolver.Log.Trace($"rightButton.PressEnded");
                    displayController.UpdateHeater(false);
                    if (Hardware.Heater is { } heater)
                    {
                        heater.IsOn = false;
                    }

                    try
                    {
                        var cl = Resolver.Services.Get<CloudLogger>();
                        cl.LogEvent(110, "relay change", new Dictionary<string, object>()
                        {
                            { "IsHeaterOn", false }
                        });
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Info($"Err: {ex.Message}");
                    }
                };
            }
        }

        private void InitializeWifi()
        {
            var wifi = Resolver.Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            if (wifi.IsConnected)
            {
                displayController?.UpdateWifi(true);
            }
            wifi.NetworkConnected += (networkAdapter, networkConnectionEventArgs) =>
            {
                Resolver.Log.Info($"Joined network - IP Address: {networkAdapter.IpAddress}");
                displayController?.UpdateWifi(true);
                //_ = audio?.PlaySystemSound(SystemSoundEffect.Chime);
            };
            wifi.NetworkDisconnected += sender =>
            {
                displayController?.UpdateWifi(false);
            };
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
            var bmeTask = Hardware.EnvironmentalSensor?.Read();
            var moistureTask = Hardware.MoistureSensor?.Read();

            await Task.WhenAll(bmeTask, moistureTask);

            var climate = new GreenhouseModel()
            {
                Temperature = (Temperature)(bmeTask?.Result.Temperature),
                Humidity = (RelativeHumidity)bmeTask?.Result.Humidity,
                SoilMoisture = moistureTask?.Result ?? 0
            };

            return climate;
        }

        private void SubscribeToCommands()
        {
            Resolver.CommandService?.Subscribe<Fan>(c =>
            {
                Resolver.Log.Info($"Received fan control: {c.IsOn}");
                displayController.UpdateVents(c.IsOn);
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
                displayController.UpdateWater(c.IsOn);
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