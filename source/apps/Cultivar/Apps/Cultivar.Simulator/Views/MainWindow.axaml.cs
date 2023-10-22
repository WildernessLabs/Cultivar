using Avalonia.Controls;
using Cultivar.Controllers;
using Cultivar.Hardware;
using ProjectLabSimulator.Displays;
using ProjectLabSimulator.ViewModels;

namespace ProjectLabSimulator.Views
{
    public partial class MainWindow : Window
    {
        private GreenhouseController greenhouseController;
        SimulatedHardware greenhouseHardware;

        PixelCanvas canvas;

        readonly int scale = 2;

        readonly ISimulatedDisplay simDisplay;

        public MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            greenhouseHardware = new SimulatedHardware();
            simDisplay = new Ili9341Simulated();

            ReloadCanvas();

            greenhouseController = new GreenhouseController(greenhouseHardware, true);

            greenhouseController.Run();
        }

        void ReloadCanvas()
        {
            displayBorder.Child = null;

            canvas = new PixelCanvas(simDisplay.Width, simDisplay.Height, simDisplay.ColorMode)
            {
                Width = simDisplay.Width * scale,
                Height = simDisplay.Height * scale,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                EnabledColor = simDisplay.ForegroundColor,
                DisabledColor = simDisplay.BackgroundColor,
            };

            //hacky ... ToDo
            greenhouseHardware.Display = canvas;


            displayBorder.Child = canvas;
            displayBorder.Width = canvas.Width;

        }

        public override void Show()
        {
            canvas?.InvalidateVisual();
            base.Show();
        }
    }
}