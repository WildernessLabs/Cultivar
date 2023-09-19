using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.UI;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Foundation.Relays;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Sensors.Buttons;
using RelayControl.UI;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using static Meadow.Foundation.Relays.ElectromagneticRelayModule;
using static System.Net.Mime.MediaTypeNames;

namespace RelayControl
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        IProjectLabHardware projectLab;
        DisplayScreen screen;
        RelayControlScreen _menu;

        TextDisplayMenu relayMenu;
        ElectromagneticRelayModule relayModule;

        public override Task Initialize()
        {
            // create the project lab hardware
            Resolver.Log.Info("Loading project lab hardware...");
            projectLab = ProjectLab.Create();

            //screen = new DisplayScreen(projectLab.Display, RotationType._270Degrees);

            // instantiate the relay board
            Resolver.Log.Info("Loading relay board...");
            relayModule = new ElectromagneticRelayModule(Device.CreateI2cBus(), 0x20);

            // load our relay text display menu
            LoadTextDisplayRelayMenuScreen();

            return base.Initialize();
        }

        void LoadTextDisplayRelayMenuScreen()
        {
            // loading from JSON
            Resolver.Log.Info("Loading menu...");
            var menuData = LoadResource("RelayMenu.json");
            Resolver.Log.Info("Menu loaded...");

            // create the graphics canvas (MicroGraphics) and set a font
            // this creates an ITextDisplay compatible object
            var graphicsCanvas = new MicroGraphics(projectLab.Display) {
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
            projectLab.DownButton.Clicked += (s, e) => {
                Resolver.Log.Info("Down Clicked.");
                relayMenu.Next();
            };
            projectLab.RightButton.Clicked += (s, e) => {
                Resolver.Log.Info("Right Clicked.");
                relayMenu.Select();
            };
            projectLab.UpButton.Clicked += (s, e) => {
                Resolver.Log.Info("Up Clicked.");
                relayMenu.Previous();
            };

        }

        private void RelayMenu_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Resolver.Log.Info($"menu item '{e.ItemID}' changed to {e.Value}");

            int relayIndex = e.ItemID switch
            {
                "relay1" => 1,
                "relay2" => 2,
                "relay3" => 3,
                "relay4" => 4,
                _ => 0,
            };

            bool intendedState = e.Value switch
            {
                "On" => true,
                "Off" => false,
                _ => false,
            };

            Resolver.Log.Info($"Turning relay {relayIndex} to {e.Value}.");
            if (relayIndex > 0) { relayModule.Relays[relayIndex].IsOn = intendedState; }
            
        }

        private void RelayMenu_Selected(object sender, MenuSelectedEventArgs e)
        {
            Resolver.Log.Info("menu selected: " + e.Command);
        }

        void ShowMicroLayoutMenuScreen()
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

            projectLab.UpButton.Clicked += (s, e) => _menu.Up();
            projectLab.DownButton.Clicked += (s, e) => _menu.Down();
        }

        public async override Task Run()
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

        byte[] LoadResource(string filename)
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