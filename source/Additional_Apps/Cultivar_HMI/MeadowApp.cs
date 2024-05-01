using Meadow;
using Meadow.Foundation.Displays;

namespace Cultivar_HMI;

public class MeadowApp : App<Desktop>
{
    private SilkDisplay display;
    private DisplayController displayController;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        display = new SilkDisplay(320, 240);
        displayController = new DisplayController(display);

        _ = displayController.Run();

        return Task.CompletedTask;
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

    public static async Task Main(string[] args)
    {
        await MeadowOS.Start(args);
    }
}