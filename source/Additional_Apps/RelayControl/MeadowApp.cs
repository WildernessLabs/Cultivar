using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays.UI;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Foundation.Relays;
using Meadow.Peripherals.Relays;
using RelayControl.UI;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RelayControl
{
    // Change ProjectLabCoreComputeApp to ProjectLabFeatherApp for V1.x boards
    public class MeadowApp : ProjectLabCoreComputeApp
    {
        private readonly DisplayScreen screen;
        private RelayControlScreen _menu;
        private TextDisplayMenu relayMenu;
        private ElectromagneticRelayModule relayModule;

        public override Task Initialize()
        {
            // create the project lab hardware
            Resolver.Log.Info("Loading project lab hardware...");

            //screen = new DisplayScreen(projectLab.Display, RotationType._270Degrees);

            // instantiate the relay board
            Resolver.Log.Info("Loading relay board...");
            relayModule = new ElectromagneticRelayModule(Hardware.Qwiic.I2cBus, 0x25);

            // load our relay text display menu
            LoadTextDisplayRelayMenuScreen();

            return base.Initialize();
        }

        private void LoadTextDisplayRelayMenuScreen()
        {
            // loading from JSON
            Resolver.Log.Info("Loading menu...");
            var menuData = LoadResource("RelayMenu.json");
            Resolver.Log.Info("Menu loaded...");

            // create the graphics canvas (MicroGraphics) and set a font
            // this creates an ITextDisplay compatible object
            var graphicsCanvas = new MicroGraphics(Hardware.Display)
            {
                CurrentFont = new Font12x20()
            };

            // create the menu
            Resolver.Log.Info("Creating menu...");
            relayMenu = new TextDisplayMenu(graphicsCanvas, menuData, false);

            // setup the selected event
            relayMenu.Selected += RelayMenu_Selected;

            relayMenu.ValueChanged += RelayMenu_ValueChanged;

            // setup the button handlers to drive the menu
            Resolver.Log.Info("Setting up button handlers.");
            Hardware.DownButton.Clicked += (s, e) =>
            {
                Resolver.Log.Info("Down Clicked.");
                relayMenu.Next();
            };
            Hardware.RightButton.Clicked += (s, e) =>
            {
                Resolver.Log.Info("Right Clicked.");
                relayMenu.Select();
            };
            Hardware.UpButton.Clicked += (s, e) =>
            {
                Resolver.Log.Info("Up Clicked.");
                relayMenu.Previous();
            };
            Hardware.LeftButton.Clicked += (s, e) =>
            {
                Resolver.Log.Info("Left Clicked.");
                relayMenu.Back();
            };
        }

        private void RelayMenu_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Resolver.Log.Info($"menu item '{e.ItemID}' changed to {e.Value}");

            int relayIndex = e.ItemID switch
            {
                "relay1" => 0,
                "relay2" => 1,
                "relay3" => 2,
                "relay4" => 3,
                _ => -1,
            };

            RelayState intendedState = e.Value switch
            {
                "On" => RelayState.Closed,
                "Off" => RelayState.Open,
                _ => RelayState.Open,
            };

            Resolver.Log.Info($"Turning relay {relayIndex + 1} to {e.Value}/{intendedState}.");
            if (relayIndex >= 0)
            {
                relayModule.Relays[relayIndex].State = intendedState;
            }
        }

        private void RelayMenu_Selected(object sender, MenuSelectedEventArgs e)
        {
            Resolver.Log.Info("menu selected: " + e.Command);
        }

        private void ShowMicroLayoutMenuScreen()
        {
            var menuItems = new string[]
                {
                    "Item A",
                    "Item B",
                    "Item C",
                    "Item D",
                    "Item E",
                    "Item F",
                };

            _menu = new RelayControlScreen(screen);

            Hardware.UpButton.Clicked += (s, e) => _menu.Up();
            Hardware.DownButton.Clicked += (s, e) => _menu.Down();
        }

        public override async Task Run()
        {
            //SplashScreen.Show(screen);

            //await Task.Delay(2000);

            //Resolver.Log.Info("splash screen going down.");

            //screen.Controls.Clear();

            //Resolver.Log.Info("cleared screen.");

            //ShowMicroLayoutMenuScreen();

            Resolver.Log.Info("Enabling menu.");
            relayMenu.Enable();

            Resolver.Log.Info("Run() returning.");
            return;
        }

        private byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"RelayControl.{filename}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}