using Azure.DigitalTwins.Core;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultivar_reTerminal.Client
{
    public static class DigitalTwinClient
    {
        public static async Task GetDigitalTwinData()
        {
            string clientId = "b441038c-fa59-42ed-8788-d82dfdc49ca8";
            string clientSecret = "WYx8Q~tz8zv.llfGodsrp3N5OH1kjB3ccKWsva9u";
            string tenantId = "3c5937be-c634-450c-beff-cce3ccfb6330";
            string endpoint = "https://cultivardemo.api.wcus.digitaltwins.azure.net"; // Replace with your instance endpoint

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            DigitalTwinsClient digitalTwinsClient = new DigitalTwinsClient(new Uri(endpoint), clientSecretCredential);

            string twinId = "23-00-3D-00-15-50-33-4D-35-34-33-20"; // Replace with the ID of the twin you want to query

            // Define the properties for your custom digital twin instance.
            var twinProperties = new Dictionary<string, object>
            {
                { "Temperature", 0 },
                { "Humidity", 0 },
                { "SoilMoisture", 0 },
                { "IsLightOn", false },
                { "IsHeaterOn", false },
                { "IsSprinklerOn", false },
                { "IsVentilationOn", false }
            };

            // Query the twin by ID
            var twin = await digitalTwinsClient.GetDigitalTwinAsync<BasicDigitalTwin>(twinId);

            // Access the twin's properties
            Console.WriteLine($"Twin ID: {twin.Value.Id}");
            //Console.WriteLine($"Twin Type: {twin.Value.Metadata.Type}");
            //Console.WriteLine($"Twin Properties: {twin.Contents}");
        }

    }
}
