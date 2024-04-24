using Cultivar_reTerminal.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cultivar_reTerminal.Client
{
    public static class RestClient
    {
        static string meadowCloudUri = "https://staging.meadowcloud.dev/";
        static string apiKey = "mcstg_WtFpp060LyIOEaQR6K7ZuL9R1H_d8V9t7c6hIKNfKEsK9g6tsHEumPABGir07XN2W259i";
        static string organizationId = "5b1d2b0dab744a04b79b245d881e18b8";
        static string deviceId = "37-00-36-00-0D-50-4B-55-30-38-31-20";

        static RestClient() { }

        public static async Task<bool> SendCommand(CultivarCommands command, bool relayState)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"apikey {apiKey}");
                    client.Timeout = new TimeSpan(0, 5, 0);

                    string jsonString = $"" +
                        $"{{" +
                            $"\"deviceIds\": [" +
                                $"\"{deviceId}\"]," +
                            $"\"commandName\": \"{command}\"," +
                            $"\"args\": {{     " +
                                $"\"relaystate\": {relayState}" +
                            $"}}," +
                            $"\"qos\": 0" +
                        $"}}";

                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{meadowCloudUri}/api/devices/commands", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response Content: " + responseContent);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                    }

                    return true;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request timed out.");
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                    return false;
                }
            }
        }


        //https://staging.meadowcloud.dev/api/v1/orgs/5b1d2b0dab744a04b79b245d881e18b8/search/source:event deviceId:27-00-1E-00-0D-50-4B-55-30-38-31-20 eventId:110 size:100`
        public static async Task<List<QueryResponse>> GetSensorReadings()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"apikey {apiKey}");
                    client.Timeout = new TimeSpan(0, 5, 0);

                    HttpResponseMessage response = await client.GetAsync($"{meadowCloudUri}/api/orgs/{organizationId}/search/source:event deviceId:{deviceId} eventId:110 size:100");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response Content: " + jsonString);

                        var root = JsonSerializer.Deserialize<Root>(jsonString);

                        return root?.data?.queryResponses;
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                        return new List<QueryResponse>();
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request timed out.");
                    return new List<QueryResponse>();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                    return new List<QueryResponse>();
                }
            }
        }
    }
}