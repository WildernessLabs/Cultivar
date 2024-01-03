using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Relays;
using Meadow.Foundation.Sensors.Moisture;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Relays;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Peripherals.Sensors.Moisture;
using Meadow.Peripherals.Speakers;
using Meadow.Units;
using System;

namespace Cultivar.Hardware
{
    public class ProductionBetaHardware : IGreenhouseHardware
    {
        protected ElectromagneticRelayModule? RelayModule { get; set; }

        public IRelay? VentFan { get; protected set; }

        public IRelay? Heater { get; protected set; }

        public IRelay? IrrigationLines { get; protected set; }

        public IRelay? Lights { get; protected set; }

        protected IProjectLabHardware projectLab { get; set; }

        public ITemperatureSensor? TemperatureSensor => projectLab.EnvironmentalSensor;

        public IHumiditySensor? HumiditySensor => projectLab.EnvironmentalSensor;

        public IToneGenerator? Speaker => projectLab.Speaker;

        public IRgbPwmLed? RgbLed => projectLab.RgbLed;

        public IButton? LeftButton => projectLab.LeftButton;

        public IButton? RightButton => projectLab.RightButton;

        public IButton? UpButton => projectLab.UpButton;

        public IButton? DownButton => projectLab.DownButton;

        public IGraphicsDisplay? Display => projectLab.Display;

        public IMoistureSensor MoistureSensor { get; set; }

        public ProductionBetaHardware()
        {
            projectLab = ProjectLab.Create();

            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            Resolver.Log.Info("Loading relay board...");
            byte relayAddress = ElectromagneticRelayModule.GetAddressFromPins(false, false, true);
            Resolver.Log.Info($"Relay address: {relayAddress:x}");

            try
            {
                RelayModule = new ElectromagneticRelayModule(projectLab.Qwiic.I2cBus, relayAddress);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Could not instantiate relay: {ex.Message}");
            }

            if (RelayModule is { } rm)
            {
                VentFan = rm.Relays[0];
                Heater = rm.Relays[1];
                Lights = rm.Relays[2];
                IrrigationLines = rm.Relays[3];
            }

            Resolver.Log.Info($"Creating the capacitive moisture sensor");

            MoistureSensor = new Capacitive(
                projectLab.IOTerminal.Pins.A1,
                minimumVoltageCalibration: new Voltage(2.84f),
                maximumVoltageCalibration: new Voltage(1.63f)
            );

            Resolver.Log.Info($"Success!");
        }
    }
}