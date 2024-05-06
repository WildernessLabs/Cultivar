using Meadow;
using Meadow.Foundation.Displays;

namespace Cultivar_HMI;

public class MeadowApp : App<Desktop>
{
    public static double VERSION { get; set; } = 1.2;

    private DisplayController displayController;

    public override async Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        Device.Display!.Resize(320, 240, 2);
        displayController = new DisplayController(Device.Display);
        displayController.ShowSplashScreen();
        Thread.Sleep(3000);
        displayController.ShowDataScreen();

        _ = displayController.Run();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

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