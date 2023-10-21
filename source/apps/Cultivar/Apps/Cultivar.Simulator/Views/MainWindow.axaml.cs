using Avalonia.Controls;
using Cultivar;
using Cultivar.Hardware;
using Cultivar.MeadowApp;
using Meadow.Foundation.Graphics;
using ProjectLabSimulator.Displays;
using ProjectLabSimulator.ViewModels;

namespace ProjectLabSimulator.Views
{
    public partial class MainWindow : Window
    {
        private GreenhouseController greenhouseController;
        SimulatedHardware greenhouseHardware;

        MicroGraphics graphics;

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

         //   graphics = new MicroGraphics(canvas)
        //    {
        //        CurrentFont = new Font8x12(),
        //    };

            displayBorder.Child = canvas;
            displayBorder.Width = canvas.Width;

        }

        void Draw()
        {
 

            /*
            graphics.Clear();

            graphics.DrawText(10, 10, "Hello MicroGraphics", Meadow.Foundation.Color.White, ScaleFactor.X1);

            graphics.DrawRectangle(10, 30, 30, 30, Meadow.Foundation.Color.White, false);

            graphics.DrawCircle(100, 100, 30, Meadow.Foundation.Color.Yellow, true);
            graphics.DrawCircle(110, 100, 30, Meadow.Foundation.Color.YellowGreen, true);
            graphics.DrawCircle(120, 100, 30, Meadow.Foundation.Color.GreenYellow, true);
            graphics.DrawCircle(130, 100, 30, Meadow.Foundation.Color.Green, true);

            graphics.Show();
            */
        }

        public override void Show()
        {
            canvas?.InvalidateVisual();
            base.Show();
        }
    }
}