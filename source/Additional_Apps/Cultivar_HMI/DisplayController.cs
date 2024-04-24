using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System;
using System.Threading.Tasks;

namespace Cultivar_HMI
{
    public class DisplayController
    {
        private readonly Image imgWifi = Image.LoadFromResource("Cultivar_HMI.Resources.img-wifi.bmp");
        private readonly Image imgSync = Image.LoadFromResource("Cultivar_HMI.Resources.img-sync.bmp");
        private readonly Image imgCloud = Image.LoadFromResource("Cultivar_HMI.Resources.img-cloud.bmp");
        private readonly Image imgWifiFade = Image.LoadFromResource("Cultivar_HMI.Resources.img-wifi-fade.bmp");
        private readonly Image imgSyncFade = Image.LoadFromResource("Cultivar_HMI.Resources.img-sync-fade.bmp");
        private readonly Image imgCloudFade = Image.LoadFromResource("Cultivar_HMI.Resources.img-cloud-fade.bmp");

        private readonly Color backgroundColor = Color.FromHex("10485E");
        private readonly Color foregroundColor = Color.White;
        private readonly Color temperatureColor = Color.FromHex("B35E2C");
        private readonly Color humidityColor = Color.FromHex("1A80AA");
        private readonly Color soilMoistureColor = Color.FromHex("737D45");
        private readonly Color sensorColor = Color.White;
        private readonly Color activeColor = Color.FromHex("14B069");
        private readonly Color inactiveColor = Color.FromHex("FF3535");

        private readonly Font12x20 font12X20 = new Font12x20();
        private readonly Font8x12 font8x12 = new Font8x12();
        private readonly Font16x24 font16x24 = new Font16x24();

        private readonly DisplayScreen screen;

        private Label StatusLabel;
        private Label CounterLabel;
        private Label TemperatureLabel;
        private Label HumidityLabel;
        private Label SoilMoistureLabel;
        private Picture wifi;
        private Picture cloud;
        private Picture sync;

        private Circle lightsCircle;
        private Circle ventsCircle;
        private Circle waterCircle;
        private Circle heaterCircle;

        public DisplayController(IPixelDisplay _display)
        {
            screen = new DisplayScreen(_display)
            {
                BackgroundColor = backgroundColor
            };

            LoadStatusBar();

            LoadCounter();

            LoadTemperatureIndicator();

            LoadHumidityIndicator();

            LoadSoilMoistureIndicator();

            LoadLightsStatus();

            LoadVentsStatus();

            LoadWaterStatus();

            LoadHeaterStatus();
        }

        private void LoadStatusBar()
        {
            int boxX = 0;
            int boxY = 0;
            int boxWidth = 320;
            int boxHeight = 30;

            StatusLabel = new Label(boxX + 5, boxY, boxWidth, boxHeight)
            {
                Text = "Connected",
                Font = font12X20,
                TextColor = foregroundColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            screen.Controls.Add(StatusLabel);

            wifi = new Picture(286, boxY + 5, imgWifiFade.Width, imgWifiFade.Height, imgWifiFade);
            screen.Controls.Add(wifi);

            cloud = new Picture(252, boxY + 5, imgCloudFade.Width, imgCloudFade.Height, imgCloudFade);
            screen.Controls.Add(cloud);

            sync = new Picture(226, boxY + 5, imgSyncFade.Width, imgSyncFade.Height, imgSyncFade);
            screen.Controls.Add(sync);
        }

        private void LoadCounter()
        {
            screen.Controls.Add(new Box(160, 5, 60, 21)
            {
                ForeColor = Color.FromHex("082936")
            });
            CounterLabel = new Label(160, 8, 60, 18)
            {
                Text = "000000",
                Font = font8x12,
                TextColor = foregroundColor,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            screen.Controls.Add(CounterLabel);
        }

        private void LoadTemperatureIndicator()
        {
            int boxX = 0;
            int boxY = 30;
            int boxWidth = 320;
            int boxHeight = 42;

            screen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
            {
                ForeColor = temperatureColor
            });

            screen.Controls.Add(new Label(boxX + 5, boxY, boxWidth - 10, boxHeight)
            {
                Text = "Temperature",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });

            TemperatureLabel = new Label(boxX + 5, boxY + 2, boxWidth - 10, boxHeight)
            {
                Text = "0",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            screen.Controls.Add(TemperatureLabel);
        }

        private void LoadHumidityIndicator()
        {
            int boxX = 0;
            int boxY = 72;
            int boxWidth = 320;
            int boxHeight = 42;

            screen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
            {
                ForeColor = humidityColor
            });

            screen.Controls.Add(new Label(boxX + 5, boxY, boxWidth - 10, boxHeight)
            {
                Text = "Humidity",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });

            HumidityLabel = new Label(boxX + 5, boxY + 2, boxWidth - 10, boxHeight)
            {
                Text = "0",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            screen.Controls.Add(HumidityLabel);
        }

        private void LoadSoilMoistureIndicator()
        {
            int boxX = 0;
            int boxY = 114;
            int boxWidth = 320;
            int boxHeight = 42;

            screen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
            {
                ForeColor = soilMoistureColor
            });

            screen.Controls.Add(new Label(boxX + 5, boxY, boxWidth - 10, boxHeight)
            {
                Text = "Soil Moisture",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });

            SoilMoistureLabel = new Label(boxX + 5, boxY + 2, boxWidth - 10, boxHeight)
            {
                Text = "0",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            screen.Controls.Add(SoilMoistureLabel);
        }

        private void LoadLightsStatus()
        {
            int boxX = 0;
            int boxY = 156;
            int boxWidth = 160;
            int boxHeight = 42;

            lightsCircle = new Circle(139, 177, 12)
            {
                ForeColor = inactiveColor,
            };
            screen.Controls.Add(lightsCircle);

            screen.Controls.Add(new Circle(135, 173, 4)
            {
                ForeColor = Color.FromHex("DCDCDC"),
            });

            screen.Controls.Add(new Label(boxX + 10, boxY + 2, boxWidth - 10, boxHeight)
            {
                Text = "Lights",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });
        }

        private void LoadVentsStatus()
        {
            int boxX = 160;
            int boxY = 156;
            int boxWidth = 160;
            int boxHeight = 42;

            screen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
            {
                ForeColor = Color.FromHex("082936")
            });

            ventsCircle = new Circle(299, 177, 12)
            {
                ForeColor = inactiveColor,
            };
            screen.Controls.Add(ventsCircle);

            screen.Controls.Add(new Circle(295, 173, 4)
            {
                ForeColor = Color.FromHex("DCDCDC"),
            });

            screen.Controls.Add(new Label(boxX + 10, boxY + 2, boxWidth - 10, boxHeight)
            {
                Text = "Vents",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });
        }

        private void LoadWaterStatus()
        {
            int boxX = 0;
            int boxY = 198;
            int boxWidth = 160;
            int boxHeight = 42;

            screen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
            {
                ForeColor = Color.FromHex("082936")
            });

            waterCircle = new Circle(139, 219, 12)
            {
                ForeColor = inactiveColor,
            };
            screen.Controls.Add(waterCircle);

            screen.Controls.Add(new Circle(135, 215, 4)
            {
                ForeColor = Color.FromHex("DCDCDC")
            });

            screen.Controls.Add(new Label(boxX + 10, boxY + 2, boxWidth - 10, boxHeight)
            {
                Text = "Water",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });
        }

        private void LoadHeaterStatus()
        {
            int boxX = 160;
            int boxY = 198;
            int boxWidth = 160;
            int boxHeight = 42;

            heaterCircle = new Circle(299, 219, 12)
            {
                ForeColor = inactiveColor,
            };
            screen.Controls.Add(heaterCircle);

            screen.Controls.Add(new Circle(295, 215, 4)
            {
                ForeColor = Color.FromHex("DCDCDC"),
            });

            screen.Controls.Add(new Label(boxX + 10, boxY + 2, boxWidth - 10, boxHeight)
            {
                Text = "Heater",
                Font = font16x24,
                TextColor = sensorColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });
        }

        public void UpdateConnectionStatus(bool connected)
        {
            wifi.Image = connected ? imgWifi : imgWifiFade;
        }

        public void UpdateCloudStatus(bool IsConnected)
        {
            cloud.Image = IsConnected ? imgCloud : imgCloudFade;
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
            lightsCircle.ForeColor = on
                ? activeColor
                : inactiveColor;
        }

        public void UpdateVents(bool on)
        {
            ventsCircle.ForeColor = on
                ? activeColor
                : inactiveColor;
        }

        public void UpdateWater(bool on)
        {
            waterCircle.ForeColor = on
                ? activeColor
                : inactiveColor;
        }

        public void UpdateHeater(bool on)
        {
            heaterCircle.ForeColor = on
                ? activeColor
                : inactiveColor;
        }

        public void UpdateReadings(int logId, double temp, double humidity, double moisture)
        {
            screen.BeginUpdate();

            CounterLabel.Text = $"{logId:D6}";
            TemperatureLabel.Text = $"{temp.ToString("N0")}°C";
            HumidityLabel.Text = $"{humidity.ToString("N0")}%";
            SoilMoistureLabel.Text = $"{moisture.ToString("N0")}%";

            screen.EndUpdate();
        }

        public async Task Run()
        {
            var random = new Random();
            bool status = false;

            while (true)
            {
                UpdateConnectionStatus(status);
                UpdateCloudStatus(status);
                UpdateSync(status);
                status = !status;

                UpdateLights(status);
                UpdateHeater(status);
                UpdateWater(status);
                UpdateVents(status);

                UpdateReadings(random.Next(20, 25), random.Next(20, 25), random.Next(20, 25), random.Next(20, 25));

                await Task.Delay(1000);
            }
        }
    }
}
