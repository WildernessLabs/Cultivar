using Meadow;
using Meadow.Foundation.Displays;
using System.Threading.Tasks;

namespace Cultivar_HMI;

public class MeadowApp : App<Windows>
{
    private SilkDisplay display;
    private DisplayController displayController;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        display = new SilkDisplay(320, 240);
        displayController = new DisplayController(display);

        _ = displayController.Run();

        return base.Initialize();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        display.Run();

        return base.Run();
    }

    public static async Task Main(string[] args)
    {
        await MeadowOS.Start(args);
    }
}