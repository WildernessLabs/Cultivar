using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;

namespace Cultivar.MeadowApp.Controllers
{
    public class DisplayController
    {
        private readonly DisplayScreen screen;

        private readonly Image imgWifi = Image.LoadFromResource("Cultivar.Assets.img-wifi.bmp");
        private readonly Image imgSync = Image.LoadFromResource("Cultivar.Assets.img-sync.bmp");
        private readonly Image imgWifiFade = Image.LoadFromResource("Cultivar.Assets.img-wifi-fade.bmp");
        private readonly Image imgSyncFade = Image.LoadFromResource("Cultivar.Assets.img-sync-fade.bmp");
        private readonly Image imgRed = Image.LoadFromResource("Cultivar.Assets.img-red.bmp");
        private readonly Image imgGreen = Image.LoadFromResource("Cultivar.Assets.img-green.bmp");

        protected Label StatusLabel { get; set; }

        protected Label TemperatureLabel { get; set; }

        protected Label HumidityLabel { get; set; }

        protected Label SoilMoistureLabel { get; set; }

        protected Picture ledLights { get; set; }

        protected Picture wifi { get; set; }

        protected Picture sync { get; set; }

        protected Picture ledVents { get; set; }

        protected Picture ledWater { get; set; }

        protected Picture ledHeater { get; set; }

        public DisplayController(IPixelDisplay _display, RotationType rotation)
        {
            screen = new DisplayScreen(_display, rotation);

            screen.Controls.Add(new Box(0, 0, screen.Width, screen.Height) { ForeColor = Color.White });
            screen.Controls.Add(new Box(0, 27, 106, 93) { ForeColor = Color.FromHex("#B35E2C") });
            screen.Controls.Add(new Box(106, 27, 108, 93) { ForeColor = Color.FromHex("#1A80AA") });
            screen.Controls.Add(new Box(214, 27, 106, 93) { ForeColor = Color.FromHex("#98A645") });

            screen.Controls.Add(new Box(160, 120, 1, screen.Height) { ForeColor = Color.Black, IsFilled = false });
            screen.Controls.Add(new Box(0, 180, screen.Width, 1) { ForeColor = Color.Black, IsFilled = false });

            StatusLabel = new Label(2, 6, 12, 16)
            {
                Text = "Hello",
                Font = new Font12x20(),
                TextColor = Color.Black
            };
            screen.Controls.Add(StatusLabel);

            wifi = new Picture(286, 3, 30, 21, imgWifiFade);
            screen.Controls.Add(wifi);

            sync = new Picture(260, 3, 21, 21, imgSyncFade);
            screen.Controls.Add(sync);

            screen.Controls.Add(new Label(5, 32, 12, 16)
            {
                Text = "Temp",
                Font = new Font12x16(),
                TextColor = Color.White
            });
            screen.Controls.Add(new Label(77, 99, 12, 16)
            {
                Text = "°C",
                Font = new Font12x20(),
                TextColor = Color.White
            });

            screen.Controls.Add(new Label(111, 32, 12, 16)
            {
                Text = "Humidity",
                Font = new Font12x16(),
                TextColor = Color.White
            });
            screen.Controls.Add(new Label(197, 99, 12, 16)
            {
                Text = "%",
                Font = new Font12x20(),
                TextColor = Color.White
            });

            screen.Controls.Add(new Label(219, 32, 12, 16)
            {
                Text = "Soil",
                Font = new Font12x16(),
                TextColor = Color.White
            });
            screen.Controls.Add(new Label(303, 99, 12, 16)
            {
                Text = "%",
                Font = new Font12x20(),
                TextColor = Color.White
            });

            TemperatureLabel = new Label(50, 70, 12, 16, ScaleFactor.X2)
            {
                Text = "0",
                Font = new Font12x16(),
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            screen.Controls.Add(TemperatureLabel);
            HumidityLabel = new Label(155, 70, 12, 16, ScaleFactor.X2)
            {
                Text = "0",
                Font = new Font12x16(),
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            screen.Controls.Add(HumidityLabel);
            SoilMoistureLabel = new Label(260, 70, 12, 16, ScaleFactor.X2)
            {
                Text = "0",
                Font = new Font12x16(),
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            screen.Controls.Add(SoilMoistureLabel);

            ledLights = new Picture(8, 128, 46, 46, imgRed)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            screen.Controls.Add(ledLights);
            screen.Controls.Add(new Label(60, 145, 12, 16, ScaleFactor.X2)
            {
                Text = "Lights",
                Font = new Font8x12(),
                TextColor = Color.Black
            });

            ledVents = new Picture(168, 128, 46, 46, imgRed)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            screen.Controls.Add(ledVents);
            screen.Controls.Add(new Label(60, 205, 12, 16, ScaleFactor.X2)
            {
                Text = "Water",
                Font = new Font8x12(),
                TextColor = Color.Black
            });

            ledWater = new Picture(8, 188, 46, 46, imgRed)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            screen.Controls.Add(ledWater);
            screen.Controls.Add(new Label(220, 145, 12, 16, ScaleFactor.X2)
            {
                Text = "Vents",
                Font = new Font8x12(),
                TextColor = Color.Black
            });

            ledHeater = new Picture(168, 188, 46, 46, imgRed)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            screen.Controls.Add(ledHeater);
            screen.Controls.Add(new Label(220, 205, 12, 16, ScaleFactor.X2)
            {
                Text = "Heater",
                Font = new Font8x12(),
                TextColor = Color.Black
            });
        }

        public void UpdateConnectionStatus(bool connected)
        {
            wifi.Image = connected ? imgWifi : imgWifiFade;
        }

        public void UpdateSync(bool on)
        {
            sync.Image = on ? imgSync : imgSyncFade;
        }

        public void UpdateStatus(string status)
        {
            StatusLabel.Text = status;
        }

        public void UpdateLights(bool on)
        {
            ledLights.Image = on ? imgGreen : imgRed;
        }

        public void UpdateHeater(bool on)
        {
            ledHeater.Image = on ? imgGreen : imgRed;
        }

        public void UpdateVents(bool on)
        {
            ledVents.Image = on ? imgGreen : imgRed;
        }

        public void UpdateWater(bool on)
        {
            ledWater.Image = on ? imgGreen : imgRed;
        }

        public void UpdateReadings(double temp, double humidity, double moisture)
        {
            screen.BeginUpdate();

            TemperatureLabel.Text = temp.ToString("N0");
            HumidityLabel.Text = humidity.ToString("N0");
            SoilMoistureLabel.Text = moisture.ToString("N0");

            screen.EndUpdate();
        }
    }
}