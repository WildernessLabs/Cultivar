using Cultivar_AzureIotHub.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Cultivar_AzureIotHub
{
    public class MeadowApp : ProjectLabCoreComputeApp
    {
        protected IotHubManager IotHubManager { get; set; }

        protected bool IsLightOn { get; set; }

        protected bool IsHeaterOn { get; set; }

        protected bool IsSprinklerOn { get; set; }

        protected bool IsVentilationOn { get; set; }

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var wifi = Hardware.ComputeModule.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += NetworkConnected;

            return base.Initialize();
        }

        private async Task EnvironmentalSensorUpdated()
        {
            var temperatureTask = Hardware.TemperatureSensor?.Read();
            var humidityTask = Hardware.HumiditySensor?.Read();

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

            IotHubManager.SendEnvironmentalReading(model);
        }

        private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Resolver.Log.Info("NetworkConnected...");

            IotHubManager = new IotHubManager();
            await IotHubManager.Initialize();

            while (true)
            {
                await EnvironmentalSensorUpdated();

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }
}