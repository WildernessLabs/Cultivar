using Meadow.Units;

namespace Cultivar.MeadowApp.Models;

public class GreenhouseModel
{
    public Temperature Temperature { get; set; }

    public RelativeHumidity Humidity { get; set; }

    public double SoilMoisture { get; set; }
}