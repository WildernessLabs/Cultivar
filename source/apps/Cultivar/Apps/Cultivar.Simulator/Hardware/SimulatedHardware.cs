using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Relays;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Moisture;
using Meadow.Peripherals.Relays;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Units;
using System;

namespace Cultivar.Hardware
{
    public class SimulatedHardware : IGreenhouseHardware
    {
        protected ElectromagneticRelayModule? RelayModule { get; set; }

        public IRelay? VentFan { get; protected set; } = null;

        public IRelay? Heater { get; protected set; } = null;

        public IRelay? IrrigationLines { get; protected set; } = null;

        public IRelay? Lights { get; protected set; } = null;

        public Bme688? EnvironmentalSensor => null;

        public PiezoSpeaker? Speaker => null;

        public RgbPwmLed? RgbLed => null;

        public IButton? LeftButton => null;

        public IButton? RightButton => null;

        public IButton? UpButton => null;

        public IButton? DownButton => null;

        public IGraphicsDisplay? Display { get; set; }

        public Capacitive? MoistureSensor { get; set; } = null;

        public SimulatedHardware()
        {
            Resolver.Services.Add(new Meadow.Logging.Logger());
            Resolver.Log.Info($"Simuated Success!");
        }
    }
}