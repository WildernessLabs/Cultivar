using Meadow.Foundation.Audio;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Moisture;
using Meadow.Peripherals.Relays;
using Meadow.Peripherals.Sensors.Buttons;

namespace Cultivar
{
    public interface IGreenhouseHardware
    {
        Capacitive MoistureSensor { get; }

        IGraphicsDisplay? Display { get; }

        RgbPwmLed? RgbLed { get; }

        Bme688? EnvironmentalSensor { get; }

        PiezoSpeaker? Speaker { get; }

        IButton? LeftButton { get; }

        IButton? RightButton { get; }

        IButton? UpButton { get; }

        IButton? DownButton { get; }

        IRelay? VentFan { get; }

        IRelay? Heater { get; }

        IRelay? IrrigationLines { get; }

        IRelay? Lights { get; }
    }
}