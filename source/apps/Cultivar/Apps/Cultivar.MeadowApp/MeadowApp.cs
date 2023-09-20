using Cultivar.MeadowApp.UI;
using Cultivar.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using Cultivar.MeadowApp.Controllers;

namespace Cultivar.MeadowApp
{
    // Change F7FeatherV2 to F7FeatherV1 if using Feather V1 Meadow boards
    // Change to F7CoreComputeV2 for Project Lab V3.x
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private IGreenhouseHardware greenhouseHardware;
        GreenhouseController GreenhouseController { get; set; }

        public override Task Initialize()
        {
            Resolver.Log.LogLevel = Meadow.Logging.LogLevel.Trace;

            Resolver.Log.Info("Initialize hardware...");

            //==== instantiate the project lab hardware
            var projLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.RevisionString}");

            greenhouseHardware = new ProductionBetaHardware(projLab);

            GreenhouseController = new GreenhouseController(greenhouseHardware);

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            GreenhouseController.Run();

            return base.Run();
        }
    }
}