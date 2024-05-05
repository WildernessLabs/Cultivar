using Cultivar.Controllers;
using Cultivar.Desktop.Hardware;
using Meadow;
using Meadow.Foundation.Displays;
using System.Threading.Tasks;

namespace Cultivar.Desktop;

public class MeadowApp : App<Meadow.Desktop>
{
    private MainController mainController;
    private SimulatedHardware greenhouseHardware;

    public override Task Initialize()
    {
        Device.Display!.Resize(320, 240, 2);

        greenhouseHardware = new SimulatedHardware()
        {
            Display = Device.Display
        };

        mainController = new MainController(greenhouseHardware, isSimulator: true);

        mainController.Run();

        return base.Initialize();
    }

    public override Task Run()
    {
        // NOTE: this will not return until the display is closed
        ExecutePlatformDisplayRunner();

        return Task.CompletedTask;
    }

    private void ExecutePlatformDisplayRunner()
    {
        if (Device.Display is SilkDisplay sd)
        {
            sd.Run();
        }
        MeadowOS.TerminateRun();
        System.Environment.Exit(0);
    }
}