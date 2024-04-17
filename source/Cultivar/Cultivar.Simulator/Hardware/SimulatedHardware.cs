using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Relays;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Relays;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Peripherals.Sensors.Moisture;
using Meadow.Peripherals.Speakers;

namespace Cultivar.Hardware
{
    public class SimulatedHardware : IGreenhouseHardware
    {
        protected ElectromagneticRelayModule? RelayModule { get; set; }

        public IRelay? VentFan { get; protected set; } = null;

        public IRelay? Heater { get; protected set; } = null;

        public IRelay? IrrigationLines { get; protected set; } = null;

        public IRelay? Lights { get; protected set; } = null;

        public ITemperatureSensor? TemperatureSensor { get; protected set; }

        public IHumiditySensor? HumiditySensor { get; protected set; }

        public IToneGenerator? Speaker { get; protected set; } = null;

        public IRgbPwmLed? RgbLed => null;

        public IButton? LeftButton => null;

        public IButton? RightButton => null;

        public IButton? UpButton => null;

        public IButton? DownButton => null;

        public IPixelDisplay? Display { get; set; }

        public IMoistureSensor? MoistureSensor { get; set; }

        public SimulatedHardware()
        {
            TemperatureSensor = new TemperatureSensorSimulated(new Meadow.Units.Temperature(20), new Meadow.Units.Temperature(-5), new Meadow.Units.Temperature(45));
            HumiditySensor = new HumiditySensorSimulated(new Meadow.Units.RelativeHumidity(50), new Meadow.Units.RelativeHumidity(0), new Meadow.Units.RelativeHumidity(100));
            MoistureSensor = new MoistureSensorSimulated(50, 5, 100);

            Resolver.Log.Info($"Simuated Success!");
        }
    }
}