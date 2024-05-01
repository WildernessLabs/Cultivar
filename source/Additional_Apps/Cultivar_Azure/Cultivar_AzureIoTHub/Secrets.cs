namespace Cultivar_AzureIotHub
{
    /// <summary>
    /// You'll need to create an IoT Hub - https://learn.microsoft.com/en-us/azure/iot-hub/iot-hub-create-through-portal
    /// Create a device within your IoT Hub
    /// And then generate a SAS token - this can be done via the Azure CLI 
    /// </summary>
    /*
     az iot hub generate-sas-token --hub-name HUB_NAME --device-id DEVICE_ID --resource-group RESOURCE_GROUP --login [Open Shared access policies -> Select iothubowner -> copy Primary connection string]
    */
    public class Secrets
    {
        /// <summary>
        /// Name of the Azure IoT Hub created
        /// </summary>
        public const string HUB_NAME = "HUB_NAME";

        /// <summary>
        /// Name of the Azure IoT Hub created
        /// </summary>
        public const string DEVICE_ID = "DEVICE_ID";

        /// <summary>
        /// example "SharedAccessSignature sr=MeadowIoTHub ..... "
        /// </summary>
        public const string SAS_TOKEN = "SAS_TOKEN";
    }
}