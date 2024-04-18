using Meadow;
using Meadow.Foundation.Displays;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cultivar_HMI;

public class MeadowApp : App<Windows>
{
    private WinFormsDisplay display;
    private DisplayController displayController;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        display = new WinFormsDisplay(320, 240);
        displayController = new DisplayController(display);

        _ = displayController.Run();

        return base.Initialize();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        Application.Run(display);

        return base.Run();
    }

    public static async Task Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        ApplicationConfiguration.Initialize();

        await MeadowOS.Start(args);
    }
}