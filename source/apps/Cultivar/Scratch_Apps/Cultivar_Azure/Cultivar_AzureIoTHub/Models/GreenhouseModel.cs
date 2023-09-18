namespace Cultivar_AzureIotHub.Models
{
    public class GreenhouseModel
    {
        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double SoilMoisture { get; set; }

        public bool IsLightOn { get; set; }

        public bool IsHeaterOn { get; set; }

        public bool IsSprinklerOn { get; set; }

        public bool IsVentilationOn { get; set; }
    }
}