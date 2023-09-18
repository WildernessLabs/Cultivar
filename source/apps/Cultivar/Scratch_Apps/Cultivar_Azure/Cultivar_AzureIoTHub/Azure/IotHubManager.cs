using Amqp;
using Amqp.Framing;
using Cultivar_AzureIotHub.Models;
using Meadow;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Cultivar_AzureIotHub
{
    /// <summary>
    /// You'll need to create an IoT Hub - https://learn.microsoft.com/en-us/azure/iot-hub/iot-hub-create-through-portal
    /// Create a device within your IoT Hub
    /// And then generate a SAS token - this can be done via the Azure CLI 
    /// 
    /// Example
    /// az iot hub generate-sas-token
    /// --hub-name HUB_NAME 
    /// --device-id DEVICE_ID 
    /// --resource-group RESOURCE_GROUP 
    /// --login [Open Shared access policies -> Select iothubowner -> copy Primary connection string]
    /// </summary>
    public class IotHubManager
    {
        private Connection connection;
        private SenderLink sender;

        public IotHubManager() { }

        public async Task Initialize()
        {
            string hostName = Secrets.HUB_NAME + ".azure-devices.net";
            string userName = Secrets.DEVICE_ID + "@sas." + Secrets.HUB_NAME;
            string senderAddress = "devices/" + Secrets.DEVICE_ID + "/messages/events";

            Resolver.Log.Info("Create connection factory...");
            var factory = new ConnectionFactory();

            Resolver.Log.Info("Create connection ...");
            connection = await factory.CreateAsync(new Address(hostName, 5671, userName, Secrets.SAS_TOKEN));

            Resolver.Log.Info("Create session ...");
            var session = new Session(connection);

            Resolver.Log.Info("Create SenderLink ...");
            sender = new SenderLink(session, "send-link", senderAddress);
        }

        public Task SendEnvironmentalReading(GreenhouseModel reading)
        {
            try
            {
                string messagePayload = $"" +
                        $"{{" +
                        $"\"Temperature\":{reading.Temperature}," +
                        $"\"Humidity\":{reading.Humidity}," +
                        $"\"SoilMoisture\":{reading.SoilMoisture}," +
                        $"\"IsLightOn\":{reading.IsLightOn}," +
                        $"\"IsHeaterOn\":{reading.IsHeaterOn}," +
                        $"\"IsSprinklerOn\":{reading.IsSprinklerOn}," +
                        $"\"IsVentilationOn\":{reading.IsVentilationOn}" +
                        $"}}";

                var payloadBytes = Encoding.UTF8.GetBytes(messagePayload);
                var message = new Message()
                {
                    BodySection = new Data() { Binary = payloadBytes }
                };

                sender.SendAsync(message);
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"-- D2C Error - {ex.Message} --");
            }

            return Task.CompletedTask;
        }
    }
}