using System;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Moisture;
using Meadow.Peripherals.Relays;

namespace Cultivar
{
	public interface IGreenhouseHardware : IProjectLabHardware
	{
        Capacitive MoistureSensor { get; }

        IRelay? VentFan { get; }
		IRelay? Heater { get;  }
		IRelay? IrrigationLines { get; }
		IRelay? Lights { get; }
	}
}