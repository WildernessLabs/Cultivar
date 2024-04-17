using Cultivar.Commands;
using Cultivar.MeadowApp.Controllers;
using Cultivar.MeadowApp.Models;
using Meadow;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Displays;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultivar.Controllers;

public class MainController
{
    private int logId = 0;

    private bool IsSampling = false;

    private bool isVentilationOn = false;
    private bool isIrrigationOn = false;
    private bool isLightOn = false;
    private bool isHeaterOn = false;

    private IGreenhouseHardware hardware;
    private INetworkAdapter? network;

    private DisplayController displayController;
    //private MicroAudio audio;

    private CloudLogger cloudLogger;

    private GreenhouseModel climate;

    private TimeSpan updateInterval = TimeSpan.FromMinutes(2);

    public MainController(IGreenhouseHardware greenhouseHardware, INetworkAdapter? networkAdapter = null, bool isSimulator = false)
    {
        hardware = greenhouseHardware;
        network = networkAdapter;

        hardware.RgbLed?.SetColor(Color.Red);

        if (hardware.Display is { } display)
        {
            displayController = new DisplayController(display, isSimulator ? RotationType.Normal : RotationType._270Degrees);
        }

        //if (Hardware.Speaker is { } speaker)
        //{
        //    speaker.SetVolume(0.5f);
        //    audio = new MicroAudio(speaker);
        //}

        if (networkAdapter != null)
        {
            WireNetworkEvents();
        }

        cloudLogger = new CloudLogger(LogLevel.Warning);
        Resolver.Log.AddProvider(cloudLogger);
        Resolver.Services.Add(cloudLogger);
        Resolver.Log.Info($"cloudlogger null? {cloudLogger is null}");

        SubscribeToCloudConnectionEvents();

        if (!isSimulator)
        {
            SubscribeToCommands();

            //HandleRelayChanges();
        }

        InitializeButtons();

        hardware.RgbLed?.SetColor(Color.Green);
        Resolver.Log.Info("Initialization complete");
    }

    private void WireNetworkEvents()
    {
        displayController.UpdateConnectionStatus(network.IsConnected);
        Resolver.Log.Info(network.IsConnected
            ? "NETWORK: Already connected."
            : "NETWORK: Not connected.");

        network.NetworkConnected += (networkAdapter, networkConnectionEventArgs) =>
        {
            Resolver.Log.Info($"NETWORK: Joined network - IP Address: {networkAdapter.IpAddress}");
            displayController.UpdateConnectionStatus(true);
            //_ = audio?.PlaySystemSound(SystemSoundEffect.Chime);
        };

        network.NetworkDisconnected += (sender, args) =>
        {
            Resolver.Log.Info($"NETWORK: Disconnected.");
            displayController.UpdateConnectionStatus(false);
        };
    }

    private void SubscribeToCloudConnectionEvents()
    {
        displayController?.UpdateStatus(Resolver.UpdateService.State.ToString());

        Resolver.MeadowCloudService.ConnectionStateChanged += (sender, state) =>
        {
            displayController?.UpdateConnectionStatus(network.IsConnected);
            displayController?.UpdateCloudStatus(state == CloudConnectionState.Connected);
            displayController?.UpdateStatus(state.ToString());
        };
    }

    private void SubscribeToCommands()
    {
        Resolver.CommandService?.Subscribe<Fan>(c =>
        {
            Resolver.Log.Info($"Received fan control: {c.IsOn}");
            displayController.UpdateVents(c.IsOn);
            if (hardware.VentFan != null)
            {
                hardware.VentFan.State = c.IsOn
                    ? Meadow.Peripherals.Relays.RelayState.Closed
                    : Meadow.Peripherals.Relays.RelayState.Open; ;
            }
        });
        Resolver.CommandService?.Subscribe<Heater>(c =>
        {
            Resolver.Log.Info($"Received heater control: {c.IsOn}");
            displayController.UpdateHeater(c.IsOn);
            if (hardware.Heater != null)
            {
                hardware.Heater.State = c.IsOn
                    ? Meadow.Peripherals.Relays.RelayState.Closed
                    : Meadow.Peripherals.Relays.RelayState.Open; ;
            }
        });
        Resolver.CommandService?.Subscribe<Lights>(c =>
        {
            Resolver.Log.Info($"Received light control: {c.IsOn}");
            displayController.UpdateLights(c.IsOn);
            if (hardware.Lights != null)
            {
                hardware.Lights.State = c.IsOn
                    ? Meadow.Peripherals.Relays.RelayState.Closed
                    : Meadow.Peripherals.Relays.RelayState.Open; ;
            }
        });
        Resolver.CommandService?.Subscribe<Irrigation>(c =>
        {
            Resolver.Log.Info($"Received valve control: {c.IsOn}");
            displayController.UpdateWater(c.IsOn);
            if (hardware.IrrigationLines != null)
            {
                hardware.IrrigationLines.State = c.IsOn
                    ? Meadow.Peripherals.Relays.RelayState.Closed
                    : Meadow.Peripherals.Relays.RelayState.Open; ;
            }
        });
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

    private void InitializeButtons()
    {
        if (hardware.UpButton is { } ventilationButton)
        {
            ventilationButton.Clicked += (s, e) =>
            {
                isVentilationOn = !isVentilationOn;

                displayController.UpdateVents(isVentilationOn);

                if (hardware.VentFan is { } ventilation)
                {
                    ventilation.State = isVentilationOn
                        ? Meadow.Peripherals.Relays.RelayState.Closed
                        : Meadow.Peripherals.Relays.RelayState.Open;
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
        if (hardware.DownButton is { } irrigationButton)
        {
            irrigationButton.Clicked += (s, e) =>
            {
                isIrrigationOn = !isIrrigationOn;

                displayController.UpdateWater(isIrrigationOn);

                if (hardware.IrrigationLines is { } irrigation)
                {
                    irrigation.State = isIrrigationOn
                        ? Meadow.Peripherals.Relays.RelayState.Closed
                        : Meadow.Peripherals.Relays.RelayState.Open;
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
        if (hardware.LeftButton is { } lightButton)
        {
            lightButton.Clicked += (s, e) =>
            {
                isLightOn = !isLightOn;

                displayController.UpdateLights(isLightOn);

                if (hardware.Lights is { } lights)
                {
                    lights.State = isLightOn
                        ? Meadow.Peripherals.Relays.RelayState.Closed
                        : Meadow.Peripherals.Relays.RelayState.Open;
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
        if (hardware.RightButton is { } heaterButton)
        {
            heaterButton.Clicked += (s, e) =>
            {
                isHeaterOn = !isHeaterOn;

                displayController.UpdateHeater(isHeaterOn);

                if (hardware.Heater is { } heater)
                {
                    heater.State = isHeaterOn
                        ? Meadow.Peripherals.Relays.RelayState.Closed
                        : Meadow.Peripherals.Relays.RelayState.Open; ;
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

    private async Task StartUpdating(TimeSpan updateInterval)
    {
        Console.WriteLine("ClimateMonitorAgent.StartUpdating()");

        if (IsSampling)
        {
            return;
        }
        IsSampling = true;

        while (IsSampling)
        {
            climate = await Read();

            Console.WriteLine($"Temperature: {climate.Temperature.Celsius:N1} | Humidity: {climate.Humidity.Percent:N1} | Moisture: {climate.SoilMoisture:N1}");

            displayController.UpdateReadings(logId, climate.Temperature.Celsius, climate.Humidity.Percent, climate.SoilMoisture);

            try
            {
                displayController.UpdateSync(true);

                var cl = Resolver.Services.Get<CloudLogger>();
                cl?.LogEvent(110, "Atmospheric reading", new Dictionary<string, object>()
                {
                    { "LogId", logId++},
                    { "TemperatureCelsius", climate.Temperature.Celsius },
                    { "HumidityPercent", climate.Humidity.Percent },
                    { "SoilMoistureDouble", climate.SoilMoisture }
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

    private async Task<GreenhouseModel> Read()
    {
        var temperatureTask = hardware.TemperatureSensor?.Read();
        var humidityTask = hardware.HumiditySensor?.Read();
        var moistureTask = hardware.MoistureSensor?.Read();

        await Task.WhenAll(temperatureTask, humidityTask, moistureTask);

        var climate = new GreenhouseModel()
        {
            Temperature = temperatureTask?.Result ?? new Meadow.Units.Temperature(0),
            Humidity = humidityTask?.Result ?? new Meadow.Units.RelativeHumidity(0),
            SoilMoisture = moistureTask?.Result ?? 0
        };

        return climate;
    }

    private void StopUpdating()
    {
        if (!IsSampling)
        {
            return;
        }

        IsSampling = false;
    }

    public Task Run()
    {
        //_ = audio.PlaySystemSound(SystemSoundEffect.Fanfare);

        _ = StartUpdating(updateInterval);

        return Task.CompletedTask;
    }
}