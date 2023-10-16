using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace Cultivar_HMI
{
    // Change F7CoreComputeV2 to F7FeatherV2 (or F7FeatherV1) for Feather boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        DisplayController displayController { get; set; }

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var projectLab = ProjectLab.Create();

            displayController = new DisplayController(projectLab.Display);

            if (projectLab.UpButton is { } upButton)
            {
                upButton.PressStarted += (s, e) =>
                {
                    displayController.UpdateVents(true);
                };
                upButton.PressEnded += (s, e) =>
                {
                    displayController.UpdateVents(false);
                };
            }
            if (projectLab.DownButton is { } downButton)
            {
                downButton.PressStarted += (s, e) =>
                {
                    displayController.UpdateWater(true);
                };
                downButton.PressEnded += (s, e) =>
                {
                    displayController.UpdateWater(false);
                };
            }
            if (projectLab.LeftButton is { } leftButton)
            {
                leftButton.PressStarted += (s, e) =>
                {
                    displayController.UpdateLights(true);
                };
                leftButton.PressEnded += (s, e) =>
                {
                    displayController.UpdateLights(false);
                };
            }
            if (projectLab.RightButton is { } rightButton)
            {
                rightButton.PressStarted += (s, e) =>
                {
                    displayController.UpdateHeater(true);
                };
                rightButton.PressEnded += (s, e) =>
                {
                    displayController.UpdateHeater(false);
                };
            }

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            _ = displayController.Run();

            return base.Run();
        }
    }
}