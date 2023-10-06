using Cultivar.Hardware;
using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private IGreenhouseHardware greenhouseHardware;
        private GreenhouseController greenhouseController;

        public override Task Initialize()
        {
            Resolver.Log.LogLevel = Meadow.Logging.LogLevel.Trace;
            Resolver.Log.Info("Initialize hardware...");

            greenhouseHardware = new ProductionBetaHardware();

            greenhouseController = new GreenhouseController(greenhouseHardware);

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            greenhouseController.Run();

            return base.Run();
        }
    }
}