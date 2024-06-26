// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cultivar_AzureFunction
{
    public static class Function1
    {
        private static readonly string adtInstanceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");
        private static readonly HttpClient singletonHttpClientInstance = new HttpClient();


        [FunctionName("IotHubToTwins")]
        public async static Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            if (adtInstanceUrl == null)
            {
                log.LogError("Application setting \"ADT_SERVICE_URL\" not set");
            }

            try
            {
                var cred = new DefaultAzureCredential();
                var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), cred);

                log.LogInformation($"ADT service client connection created");

                if (eventGridEvent != null && eventGridEvent.Data != null)
                {
                    log.LogInformation(eventGridEvent.Data.ToString());

                    var deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());

                    var decodedBytes = Convert.FromBase64String(deviceMessage["body"].ToString());
                    var jsonString = Encoding.UTF8.GetString(decodedBytes);

                    log.LogInformation(jsonString);

                    var dataMessage = (JObject)JsonConvert.DeserializeObject(jsonString);

                    // get our device id and greenhouse data
                    string deviceId = (string)deviceMessage["systemProperties"]["iothub-connection-device-id"];
                    var temperature = dataMessage["Temperature"];
                    var humidity = dataMessage["Humidity"];
                    var soilMoisture = dataMessage["SoilMoisture"];
                    var isLightOn = dataMessage["IsLightOn"];
                    var isHeaterOn = dataMessage["IsHeaterOn"];
                    var isSprinklerOn = dataMessage["IsSprinklerOn"];
                    var isVentilationOn = dataMessage["IsVentilationOn"];

                    var updateTwinData = new JsonPatchDocument();
                    updateTwinData.AppendReplace("/Temperature", temperature.Value<double>());
                    updateTwinData.AppendReplace("/Humidity", humidity.Value<double>());
                    updateTwinData.AppendReplace("/SoilMoisture", soilMoisture.Value<double>());
                    updateTwinData.AppendReplace("/IsLightOn", isLightOn.Value<bool>());
                    updateTwinData.AppendReplace("/IsHeaterOn", isLightOn.Value<bool>());
                    updateTwinData.AppendReplace("/IsSprinklerOn", isLightOn.Value<bool>());
                    updateTwinData.AppendReplace("/IsVentilationOn", isLightOn.Value<bool>());

                    await client.UpdateDigitalTwinAsync(deviceId, updateTwinData);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error in ingest function: {ex.Message}");
            }
        }
    }
}