using Cultivar.Commands;
using Cultivar.MeadowApp.Controllers;
using Cultivar.MeadowApp.Models;
using Meadow;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Relays;
using Meadow.Update;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cultivar.Controllers;

public class MainController
{
    public static double VERSION { get; set; } = 1.4;

    private int TIMEZONE_OFFSET = -7;

    private int logId = 0;

    private bool IsSampling = false;

    private bool isVentilationOn = false;
    private bool isIrrigationOn = false;
    private bool isLightOn = false;
    private bool isHeaterOn = false;

    private IGreenhouseHardware hardware;
    private INetworkAdapter? network;

    private DisplayController displayController;

    private CloudLogger cloudLogger;

    private GreenhouseModel climate;

    private TimeSpan updateInterval = TimeSpan.FromMinutes(1);

    public MainController(IGreenhouseHardware greenhouseHardware, INetworkAdapter? networkAdapter = null, bool isSimulator = false)
    {
        hardware = greenhouseHardware;
        network = networkAdapter;

        hardware.RgbLed?.SetColor(Color.Red);

        if (hardware.Display is { } display)
        {
            displayController = new DisplayController(display, isSimulator
                ? RotationType.Normal
                : RotationType._270Degrees);
            displayController.ShowSplashScreen();
            Thread.Sleep(3000);
            displayController.ShowDataScreen();
        }

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
        if (network.IsConnected)
        {
            displayController.UpdateConnectionStatus(true);
        }
        else
        {
            _ = displayController.StartConnectingWiFiAnimation();
        }

        Resolver.Log.Info(network.IsConnected
            ? "NETWORK: Already connected."
            : "NETWORK: Not connected.");

        network.NetworkConnected += (networkAdapter, networkConnectionEventArgs) =>
        {
            Resolver.Log.Info($"NETWORK: Joined network - IP Address: {networkAdapter.IpAddress}");
            displayController.UpdateConnectionStatus(true, true);
        };

        network.NetworkDisconnected += (sender, args) =>
        {
            Resolver.Log.Info($"NETWORK: Disconnected.");
            displayController.UpdateConnectionStatus(false, true);
        };
    }

    private void SubscribeToCloudConnectionEvents()
    {
        _ = displayController.StartConnectingCloudAnimation();

        displayController?.UpdateStatus(Resolver.UpdateService.State.ToString());

        var updateService = Resolver.UpdateService;
        updateService.ClearUpdates(); // uncomment to clear persisted info
        updateService.StateChanged += OnUpdateStateChanged;
        updateService.RetrieveProgress += OnUpdateProgress;
        updateService.UpdateAvailable += OnUpdateAvailable;
        updateService.UpdateRetrieved += OnUpdateRetrieved;

        Resolver.MeadowCloudService.ConnectionStateChanged += (sender, state) =>
        {
            if (state == CloudConnectionState.Connected)
            {
                displayController?.UpdateCloudStatus(true, true);
            }

            displayController?.UpdateStatus(state.ToString());
            Thread.Sleep(2000);
            displayController?.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("hh:mm tt"));
        };
    }

    private void OnUpdateStateChanged(object sender, UpdateState e)
    {
        displayController.UpdateCloudStatus($"{FormatStatusMessage(e)}");
    }

    private void OnUpdateProgress(IUpdateService updateService, UpdateInfo info)
    {
        short percentage = (short)(((double)info.DownloadProgress / info.FileSize) * 100);

        displayController.UpdateDownloadProgress(percentage);
    }

    private async void OnUpdateAvailable(IUpdateService updateService, UpdateInfo info)
    {
        _ = hardware.RgbLed.StartBlink(Color.Magenta);
        displayController.ShowUpdateScreen();
        displayController.UpdateCloudStatus("Update available!");

        await Task.Delay(5000);
        updateService.RetrieveUpdate(info);
    }

    private async void OnUpdateRetrieved(IUpdateService updateService, UpdateInfo info)
    {
        _ = hardware.RgbLed.StartBlink(Color.Cyan);
        displayController.UpdateCloudStatus("Update retrieved!");

        await Task.Delay(5000);
        updateService.ApplyUpdate(info);
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
                    ? RelayState.Closed
                    : RelayState.Open;
            }
        });
        Resolver.CommandService?.Subscribe<Heater>(c =>
        {
            Resolver.Log.Info($"Received heater control: {c.IsOn}");
            displayController.UpdateHeater(c.IsOn);
            if (hardware.Heater != null)
            {
                hardware.Heater.State = c.IsOn
                    ? RelayState.Closed
                    : RelayState.Open;
            }
        });
        Resolver.CommandService?.Subscribe<Lights>(c =>
        {
            Resolver.Log.Info($"Received light control: {c.IsOn}");
            displayController.UpdateLights(c.IsOn);
            if (hardware.Lights != null)
            {
                hardware.Lights.State = c.IsOn
                    ? RelayState.Closed
                    : RelayState.Open;
            }
        });
        Resolver.CommandService?.Subscribe<Irrigation>(c =>
        {
            Resolver.Log.Info($"Received valve control: {c.IsOn}");
            displayController.UpdateWater(c.IsOn);
            if (hardware.IrrigationLines != null)
            {
                hardware.IrrigationLines.State = c.IsOn
                    ? RelayState.Closed
                    : RelayState.Open;
            }
        });
    }

    private void HandleRelayChanges()
    {
        RegisterRelayChange(hardware.VentFan, "IsVentilationOn");
        RegisterRelayChange(hardware.Heater, "IsHeaterOn");
        RegisterRelayChange(hardware.Lights, "IsLightOn");
        RegisterRelayChange(hardware.IrrigationLines, "IsIrrigationOn");
    }

    private void RegisterRelayChange(IRelay relay, string eventName)
    {
        relay.OnChanged += (sender, relayState) =>
        {
            Resolver.Log.Trace($"relay changed, {eventName}:{relayState}");
            try
            {
                var cl = Resolver.Services.Get<CloudLogger>();
                cl?.LogEvent(110, "relay change", new Dictionary<string, object>()
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
                        ? RelayState.Closed
                        : RelayState.Open;
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
                        ? RelayState.Closed
                        : RelayState.Open;
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
                        ? RelayState.Closed
                        : RelayState.Open;
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
                        ? RelayState.Closed
                        : RelayState.Open;
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
        Resolver.Log.Info("ClimateMonitorAgent.StartUpdating()");

        if (IsSampling)
        {
            return;
        }
        IsSampling = true;

        while (IsSampling)
        {
            climate = await Read();

            Resolver.Log.Info($"Temperature: {climate.Temperature.Celsius:N1} | Humidity: {climate.Humidity.Percent:N1} | Moisture: {climate.SoilMoisture:N1}");

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

                displayController.UpdateStatus("Log Sent!");
                await Task.Delay(2000);

                displayController.UpdateSync(false);
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"Err: {ex.Message}");
            }

            displayController.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("hh:mm tt"));

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
        _ = StartUpdating(updateInterval);

        return Task.CompletedTask;
    }

    private string FormatStatusMessage(UpdateState state)
    {
        string message = string.Empty;

        switch (state)
        {
            case UpdateState.Dead: message = "Failed"; break;
            case UpdateState.Disconnected: message = "Disconnected"; break;
            case UpdateState.Connected: message = "Connected!"; break;
            case UpdateState.DownloadingFile: message = "Downloading File..."; break;
            case UpdateState.UpdateInProgress: message = "Update In Progress..."; break;
        }

        return message;
    }
}