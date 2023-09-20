using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cultivar_reTerminal.Client
{
    public static class RestClient
    {
        static string meadowCloudUri = "https://staging.meadowcloud.dev/";
        static string apiKey = "mcdev_HSAnsb2zNkw3X8Yq7tOaehpoul4_uV22i4x5dedRCCXQgawAH5Ft5U1JW4D56HK9107ThP";
        static string deviceId = "27-00-1E-00-0D-50-4B-55-30-38-31-20";

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
    }
}