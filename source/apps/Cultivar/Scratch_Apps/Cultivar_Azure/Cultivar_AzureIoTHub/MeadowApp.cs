using Cultivar_AzureIotHub.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Cultivar_AzureIotHub
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        protected IotHubManager iotHubManager { get; set; }

        protected bool IsLightOn { get; set; }

        protected bool IsHeaterOn { get; set; }

        protected bool IsSprinklerOn { get; set; }

        protected bool IsVentilationOn { get; set; }

        protected IProjectLabHardware projectLab { get; set; }

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += NetworkConnected;

            return base.Initialize();
        }

        private async Task EnvironmentalSensorUpdated()
        {
            var temperatureTask = projectLab.TemperatureSensor?.Read();
            var humidityTask = projectLab.HumiditySensor?.Read();

            await Task.WhenAll(temperatureTask, humidityTask);

            var model = new GreenhouseModel()
            {
                Temperature = temperatureTask?.Result.Celsius ?? new Meadow.Units.Temperature(0).Celsius,
                Humidity = humidityTask?.Result.Percent ?? new Meadow.Units.RelativeHumidity(0).Percent,
                SoilMoisture = humidityTask?.Result.Percent ?? new Meadow.Units.RelativeHumidity(0).Percent,
                IsLightOn = IsLightOn,
                IsHeaterOn = IsHeaterOn,
                IsSprinklerOn = IsSprinklerOn,
                IsVentilationOn = IsVentilationOn
            };

            Resolver.Log.Info($"Reading {DateTime.Now} - " +
                $"Temperature: {model.Temperature:N2}°C, " +
                $"Humidity: {model.Humidity:N2}%, " +
                $"SoilMoisture: {model.Humidity - 10:N2}%, " +
                $"IsLightOn: {IsLightOn}, " +
                $"IsHeaterOn: {IsHeaterOn}, " +
                $"IsSprinklerOn: {IsSprinklerOn}, " +
                $"IsVentilationOn: {IsVentilationOn}");

            iotHubManager.SendEnvironmentalReading(model);
        }

        private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Resolver.Log.Info("NetworkConnected...");

            iotHubManager = new IotHubManager();
            await iotHubManager.Initialize();

            projectLab = ProjectLab.Create();

            while (true)
            {
                await EnvironmentalSensorUpdated();

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }
}