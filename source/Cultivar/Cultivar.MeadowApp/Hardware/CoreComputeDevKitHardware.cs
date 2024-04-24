using Meadow.Foundation.Relays;
using Meadow.Foundation.Sensors;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Relays;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Peripherals.Sensors.Moisture;
using Meadow.Peripherals.Speakers;
using Meadow.Units;

namespace Cultivar.Hardware;

public class CoreComputeDevKitHardware : IGreenhouseHardware
{
    public IMoistureSensor? MoistureSensor { get; }
    public ITemperatureSensor? TemperatureSensor { get; }
    public IHumiditySensor? HumiditySensor { get; }
    public IRelay? VentFan { get; }
    public IRelay? Heater { get; }
    public IRelay? IrrigationLines { get; }
    public IRelay? Lights { get; }
    public IPixelDisplay? Display { get; } = null;

    public IRgbPwmLed? RgbLed => null;
    public IToneGenerator? Speaker => null;
    public IButton? LeftButton => null;
    public IButton? RightButton => null;
    public IButton? UpButton => null;
    public IButton? DownButton => null;


    public CoreComputeDevKitHardware()
    {
        // dev note: unsure what lines are connected to the 7789.  Fill these in for display support
        //Display = new St7789(

        MoistureSensor = new SimulatedMoistureSensor();
        TemperatureSensor = new SimulatedTemperatureSensor(25.Celsius(), 20.Celsius(), 28.Celsius());
        HumiditySensor = new SimulatedHumiditySensor();
        VentFan = new SimulatedRelay("Fan");
        Heater = new SimulatedRelay("Heater");
        IrrigationLines = new SimulatedRelay("Irrigation");
        Lights = new SimulatedRelay("Lights");
    }
}
