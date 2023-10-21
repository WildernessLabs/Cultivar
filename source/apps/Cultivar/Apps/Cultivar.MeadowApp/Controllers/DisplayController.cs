using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

namespace Cultivar.MeadowApp.Controllers
{
    public class DisplayController
    {
        private DisplayScreen screen;

        private Image imgWifi = Image.LoadFromResource("Cultivar.MeadowApp.img-wifi.bmp");
        private Image imgSync = Image.LoadFromResource("Cultivar.MeadowApp.img-sync.bmp");
        private Image imgWifiFade = Image.LoadFromResource("Cultivar.MeadowApp.img-wifi-fade.bmp");
        private Image imgSyncFade = Image.LoadFromResource("Cultivar.MeadowApp.img-sync-fade.bmp");
        private Image imgRed = Image.LoadFromResource("Cultivar.MeadowApp.img-red.bmp");
        private Image imgGreen = Image.LoadFromResource("Cultivar.MeadowApp.img-green.bmp");

        protected Label StatusLabel { get; set; }

        protected Label TemperatureLabel { get; set; }

        protected Label HumidityLabel { get; set; }

        protected Label SoilMoistureLabel { get; set; }

        protected Picture ledLights { get; set; }

        protected Picture wifi { get; set; }

        protected Picture sync { get; set; }

        protected Picture ledWater { get; set; }

        protected Picture ledVents { get; set; }

        protected Picture ledHeater { get; set; }

        public DisplayController(IGraphicsDisplay _display)
        {
            screen = new DisplayScreen(_display, RotationType.Normal);

            screen.Controls.Add(new Box(0, 0, screen.Width, screen.Height) { ForeColor = Meadow.Foundation.Color.White });
            screen.Controls.Add(new Box(0, 27, 106, 93) { ForeColor = Meadow.Foundation.Color.FromHex("#B35E2C") });
            screen.Controls.Add(new Box(106, 27, 108, 93) { ForeColor = Meadow.Foundation.Color.FromHex("#1A80AA") });
            screen.Controls.Add(new Box(214, 27, 106, 93) { ForeColor = Meadow.Foundation.Color.FromHex("#98A645") });

            screen.Controls.Add(new Box(160, 120, 1, screen.Height) { ForeColor = Meadow.Foundation.Color.Black, Filled = false });
            screen.Controls.Add(new Box(0, 180, screen.Width, 1) { ForeColor = Meadow.Foundation.Color.Black, Filled = false });

            StatusLabel = new Label(2, 6, 12, 16)
            {
                Text = "Hello",
                Font = new Font12x20(),
                TextColor = Meadow.Foundation.Color.Black
            };
            screen.Controls.Add(StatusLabel);

            wifi = new Picture(286, 3, 30, 21, imgWifiFade);
            screen.Controls.Add(wifi);

            sync = new Picture(260, 3, 21, 21, imgSyncFade);
            screen.Controls.Add(sync);

            screen.Controls.Add(new Label(5, 32, 12, 16)
            {
                Text = "TEMP.",
                Font = new Font12x16(),
                TextColor = Meadow.Foundation.Color.White
            });
            screen.Controls.Add(new Label(77, 99, 12, 16)
            {
                Text = "°C",
                Font = new Font12x20(),
                TextColor = Meadow.Foundation.Color.White
            });

            screen.Controls.Add(new Label(111, 32, 12, 16)
            {
                Text = "HUM.",
                Font = new Font12x16(),
                TextColor = Meadow.Foundation.Color.White
            });
            screen.Controls.Add(new Label(197, 99, 12, 16)
            {
                Text = "%",
                Font = new Font12x20(),
                TextColor = Meadow.Foundation.Color.White
            });

            screen.Controls.Add(new Label(219, 32, 12, 16)
            {
                Text = "S.M.",
                Font = new Font12x16(),
                TextColor = Meadow.Foundation.Color.White
            });
            screen.Controls.Add(new Label(303, 99, 12, 16)
            {
                Text = "%",
                Font = new Font12x20(),
                TextColor = Meadow.Foundation.Color.White
            });

            TemperatureLabel = new Label(50, 70, 12, 16, ScaleFactor.X2)
            {
                Text = "0",
                Font = new Font12x16(),
                TextColor = Meadow.Foundation.Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            screen.Controls.Add(TemperatureLabel);
            HumidityLabel = new Label(155, 70, 12, 16, ScaleFactor.X2)
            {
                Text = "0",
                Font = new Font12x16(),
                TextColor = Meadow.Foundation.Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            screen.Controls.Add(HumidityLabel);
            SoilMoistureLabel = new Label(260, 70, 12, 16, ScaleFactor.X2)
            {
                Text = "0",
                Font = new Font12x16(),
                TextColor = Meadow.Foundation.Color.White,
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
                TextColor = Meadow.Foundation.Color.Black
            });

            ledWater = new Picture(168, 128, 46, 46, imgRed)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            screen.Controls.Add(ledWater);
            screen.Controls.Add(new Label(60, 205, 12, 16, ScaleFactor.X2)
            {
                Text = "Water",
                Font = new Font8x12(),
                TextColor = Meadow.Foundation.Color.Black
            });

            ledVents = new Picture(8, 188, 46, 46, imgRed)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            screen.Controls.Add(ledVents);
            screen.Controls.Add(new Label(220, 145, 12, 16, ScaleFactor.X2)
            {
                Text = "Vents",
                Font = new Font8x12(),
                TextColor = Meadow.Foundation.Color.Black
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
                TextColor = Meadow.Foundation.Color.Black
            });
        }

        public void UpdateWifi(bool on)
        {
            wifi.Image = on ? imgWifi : imgWifiFade;
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

        public void UpdateWater(bool on)
        {
            ledWater.Image = on ? imgGreen : imgRed;
        }

        public void UpdateVents(bool on)
        {
            ledVents.Image = on ? imgGreen : imgRed;
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