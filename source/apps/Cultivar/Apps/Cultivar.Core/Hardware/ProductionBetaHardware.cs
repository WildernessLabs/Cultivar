using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Relays;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Light;
using Meadow.Foundation.Sensors.Moisture;
using Meadow.Hardware;
using Meadow.Modbus;
using Meadow.Peripherals.Relays;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Units;

namespace Cultivar.Hardware
{
    public class ProductionBetaHardware : IGreenhouseHardware
    {
        protected IProjectLabHardware ProjectLab { get; set; }
        protected ElectromagneticRelayModule RelayModule { get; set; }
        public Capacitive MoistureSensor { get; set; }

        public ProductionBetaHardware(IProjectLabHardware projectLab)
        {
            this.ProjectLab = projectLab;

            //Resolver.Log.Info($"getting the CCM ref");
            //var ccm = Resolver.Device as F7CoreComputeV2;

            // instantiate the relay board
            Resolver.Log.Info("Loading relay board...");
            byte relayAddress = ElectromagneticRelayModule.GetAddressFromPins(false, false, true);
            Resolver.Log.Info($"relay address: {relayAddress:x}");
            RelayModule = new ElectromagneticRelayModule(projectLab.Qwiic.I2cBus, relayAddress);

            // assign the relay shortcuts
            this.VentFan = RelayModule.Relays[0];
            this.Heater = RelayModule.Relays[1];
            this.Lights = RelayModule.Relays[2];
            this.IrrigationLines = RelayModule.Relays[3];


            // capacitive moisture sensor
            //if (ccm is not null)
            //{
                Resolver.Log.Info($"creating the capacitive moisture sensor");
                MoistureSensor = new Capacitive(
                    projectLab.IOTerminal.Pins.A1,
                    //ccm.Pins.A04,
                    minimumVoltageCalibration: new Voltage(2.84f),
                    maximumVoltageCalibration: new Voltage(1.63f)
                );
                Resolver.Log.Info($"success!");
            //}

        }

        public IRelay? VentFan { get; protected set; }

        public IRelay? Heater { get; protected set; }

        public IRelay? IrrigationLines { get; protected set; }

        public IRelay? Lights { get; protected set; }

        public ISpiBus SpiBus => this.ProjectLab.SpiBus;

        public II2cBus I2cBus => this.ProjectLab.I2cBus;

        public Bh1750? LightSensor => this.ProjectLab.LightSensor;

        public Bme688? EnvironmentalSensor => this.ProjectLab.EnvironmentalSensor;

        public Bmi270? MotionSensor => this.ProjectLab.MotionSensor;

        public PiezoSpeaker? Speaker => this.ProjectLab.Speaker;

        public RgbPwmLed? RgbLed => this.ProjectLab.RgbLed;

        public IButton? LeftButton => this.ProjectLab.LeftButton;

        public IButton? RightButton => this.ProjectLab.RightButton;

        public IButton? UpButton => this.ProjectLab.UpButton;

        public IButton? DownButton => this.ProjectLab.DownButton;

        public string RevisionString => this.ProjectLab.RevisionString;

        public MikroBusConnector MikroBus1 => this.ProjectLab.MikroBus1;

        public MikroBusConnector MikroBus2 => this.ProjectLab.MikroBus2;

        public GroveDigitalConnector? GroveDigital => this.ProjectLab.GroveDigital;

        public GroveDigitalConnector GroveAnalog => this.ProjectLab.GroveAnalog;

        public UartConnector GroveUart => this.ProjectLab.GroveUart;

        public I2cConnector Qwiic => this.ProjectLab.Qwiic;

        public IOTerminalConnector IOTerminal => this.ProjectLab.IOTerminal;

        public IGraphicsDisplay? Display => this.ProjectLab.Display;

        public ModbusRtuClient GetModbusRtuClient(
            int baudRate = 19200, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
                => this.ProjectLab.GetModbusRtuClient(baudRate, dataBits, parity, stopBits);
    }
}