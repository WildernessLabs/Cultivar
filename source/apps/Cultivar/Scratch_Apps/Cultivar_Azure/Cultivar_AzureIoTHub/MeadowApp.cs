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

        private void EnvironmentalSensorUpdated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> e)
        {
            var model = new GreenhouseModel()
            {
                Temperature = $"{e.New.Temperature.Value.Celsius:N2}°C",
                Humidity = $"{e.New.Humidity.Value.Percent:N2}°C",
                SoilMoisture = $"{e.New.Humidity.Value.Percent - 10:N2}°C",
                IsLightOn = IsLightOn,
                IsHeaterOn = IsHeaterOn,
                IsSprinklerOn = IsSprinklerOn,
                IsVentilationOn = IsVentilationOn
            };

            Resolver.Log.Info($"Reading {DateTime.Now} - " +
                $"Temperature: {e.New.Temperature.Value.Celsius:N2}°C, " +
                $"Humidity: {e.New.Humidity.Value.Percent:N2}%, " +
                $"SoilMoisture: {e.New.Humidity.Value.Percent - 10:N2}atm, " +
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
            projectLab.EnvironmentalSensor.Updated += EnvironmentalSensorUpdated;
            projectLab.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(30));
        }
    }
}