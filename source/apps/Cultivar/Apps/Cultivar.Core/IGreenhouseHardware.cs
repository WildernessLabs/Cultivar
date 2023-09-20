using System;
using Meadow.Devices;
using Meadow.Peripherals.Relays;

namespace Cultivar
{
	public interface IGreenhouseHardware : IProjectLabHardware
	{
		IRelay? VentFan { get; }
		IRelay? Heater { get;  }
		IRelay? IrrigationLines { get; }
		IRelay? Lights { get; }
	}
}