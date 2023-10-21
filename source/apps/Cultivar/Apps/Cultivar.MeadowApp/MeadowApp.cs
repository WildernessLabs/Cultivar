using Cultivar.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Logging;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private GreenhouseController greenhouseController;

        public override Task Initialize()
        {
            var cloudLogger = new CloudLogger(LogLevel.Warning);
            Resolver.Log.AddProvider(cloudLogger);
            Resolver.Services.Add(cloudLogger);

            Resolver.Log.Info($"cloudlogger null? {cloudLogger is null}");

            Resolver.Log.Info("Initialize hardware...");

            var greenhouseHardware = new ProductionBetaHardware();
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