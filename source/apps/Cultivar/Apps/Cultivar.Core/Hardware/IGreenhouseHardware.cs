using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Relays;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Peripherals.Sensors.Moisture;
using Meadow.Peripherals.Speakers;

namespace Cultivar;

public interface IGreenhouseHardware
{
    IMoistureSensor? MoistureSensor { get; }

    IPixelDisplay? Display { get; }

    IRgbPwmLed? RgbLed { get; }

    ITemperatureSensor? TemperatureSensor { get; }

    IHumiditySensor? HumiditySensor { get; }

    IToneGenerator? Speaker { get; }

    IButton? LeftButton { get; }

    IButton? RightButton { get; }

    IButton? UpButton { get; }

    IButton? DownButton { get; }

    IRelay? VentFan { get; }

    IRelay? Heater { get; }

    IRelay? IrrigationLines { get; }

    IRelay? Lights { get; }
}