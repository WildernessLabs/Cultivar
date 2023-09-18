namespace Cultivar_AzureIotHub.Models
{
    public class GreenhouseModel
    {
        public string Temperature { get; set; }

        public string Humidity { get; set; }

        public string SoilMoisture { get; set; }

        public bool IsLightOn { get; set; }

        public bool IsHeaterOn { get; set; }

        public bool IsSprinklerOn { get; set; }

        public bool IsVentilationOn { get; set; }
    }
}