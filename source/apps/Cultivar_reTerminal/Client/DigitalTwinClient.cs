using Azure.DigitalTwins.Core;
using Azure.Identity;
using Cultivar_reTerminal.Models;
using System;
using System.Threading.Tasks;

namespace Cultivar_reTerminal.Client
{
    public static class DigitalTwinClient
    {
        public static async Task<GreenhouseModel> GetDigitalTwinData()
        {
            string clientId = "b441038c-fa59-42ed-8788-d82dfdc49ca8";
            string clientSecret = "WYx8Q~tz8zv.llfGodsrp3N5OH1kjB3ccKWsva9u";
            string tenantId = "3c5937be-c634-450c-beff-cce3ccfb6330";
            string endpoint = "https://cultivardemo.api.wcus.digitaltwins.azure.net"; // Replace with your instance endpoint
            string twinId = "37-00-36-00-0D-50-4B-55-30-38-31-20";

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(endpoint), clientSecretCredential);

            var twin = await digitalTwinsClient.GetDigitalTwinAsync<BasicDigitalTwin>(twinId);

            var model = new GreenhouseModel();

            if (twin != null)
            {
                Console.WriteLine($"Digital Twin ID: {twin.Value.Id}");
                Console.WriteLine($"Model ID: {twin.Value.Metadata.ModelId}");

                if (twin.Value.Contents.TryGetValue("Temperature", out var temperature))
                {
                    double.TryParse(temperature.ToString(), out var dtemperature);
                    model.TemperatureCelsius = dtemperature;
                }
                if (twin.Value.Contents.TryGetValue("Humidity", out var humidity))
                {
                    double.TryParse(humidity.ToString(), out var dhumidity);
                    model.HumidityPercentage = dhumidity;
                }
                if (twin.Value.Contents.TryGetValue("SoilMoisture", out var soilMoisture))
                {
                    double.TryParse(soilMoisture.ToString(), out var dsoilMoisture);
                    model.SoilMoisturePercentage = dsoilMoisture;
                }
                if (twin.Value.Contents.TryGetValue("IsLightOn", out var isLightOn))
                {
                    bool.TryParse(isLightOn.ToString(), out var bisLightOn);
                    model.IsLightOn = bisLightOn;
                }
                if (twin.Value.Contents.TryGetValue("IsHeaterOn", out var isHeaterOn))
                {
                    bool.TryParse(isHeaterOn.ToString(), out var bisHeaterOn);
                    model.IsHeaterOn = bisHeaterOn;
                }
                if (twin.Value.Contents.TryGetValue("IsSprinklerOn", out var isSprinklerOn))
                {
                    bool.TryParse(isSprinklerOn.ToString(), out var bisSprinklerOn);
                    model.IsSprinklerOn = bisSprinklerOn;
                }
                if (twin.Value.Contents.TryGetValue("IsVentilationOn", out var isVentilationOn))
                {
                    bool.TryParse(isVentilationOn.ToString(), out var bisVentilationOn);
                    model.IsVentilationOn = bisVentilationOn;
                }
            }
            else
            {
                Console.WriteLine($"Digital Twin with ID '{twinId}' not found.");
            }

            return model;
        }
    }
}