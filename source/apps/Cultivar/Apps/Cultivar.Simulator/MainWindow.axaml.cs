using Avalonia.Controls;
using Cultivar.Controllers;
using Cultivar.Hardware;
using ProjectLabSimulator.Displays;

namespace ProjectLabSimulator.Views
{
    public partial class MainWindow : Window
    {
        private readonly GreenhouseController greenhouseController;
        readonly SimulatedHardware greenhouseHardware;

        readonly int scale = 2;

        public MainWindow()
        {
            InitializeComponent();

            greenhouseHardware = new SimulatedHardware()
            {
                Display = LoadDisplay()
            };

            greenhouseController = new GreenhouseController(greenhouseHardware, true);

            greenhouseController.Run();
        }

        PixelCanvas LoadDisplay()
        {
            var simDisplay = new Ili9341Simulated();

            var canvas = new PixelCanvas(simDisplay.Width, simDisplay.Height, simDisplay.ColorMode)
            {
                Width = simDisplay.Width * scale,
                Height = simDisplay.Height * scale,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                EnabledColor = simDisplay.ForegroundColor,
                DisabledColor = simDisplay.BackgroundColor,
            };

            displayBorder.Child = canvas;
            displayBorder.Width = canvas.Width;

            return canvas;
        }
    }
}