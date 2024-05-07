using Cultivar.Controllers;
using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cultivar.MeadowApp.Controllers;

public class DisplayController
{
    private CancellationTokenSource connectivityToken;
    private CancellationTokenSource cloudToken;

    private readonly Image imgWifi = Image.LoadFromResource("Cultivar.Assets.img-wifi.bmp");
    private readonly Image imgSync = Image.LoadFromResource("Cultivar.Assets.img-sync.bmp");
    private readonly Image imgCloud = Image.LoadFromResource("Cultivar.Assets.img-cloud.bmp");
    private readonly Image imgWifiFade = Image.LoadFromResource("Cultivar.Assets.img-wifi-fade.bmp");
    private readonly Image imgSyncFade = Image.LoadFromResource("Cultivar.Assets.img-sync-fade.bmp");
    private readonly Image imgCloudFade = Image.LoadFromResource("Cultivar.Assets.img-cloud-fade.bmp");

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
    private readonly Font12x16 font12x16 = new Font12x16();
    private readonly Font16x24 font16x24 = new Font16x24();

    private readonly DisplayScreen displayScreen;

    private AbsoluteLayout splashScreen;

    private AbsoluteLayout dataScreen;

    private Label statusLabel;
    private Picture wifi;
    private Picture cloud;
    private Picture sync;
    private Label counterLabel;
    private Label temperatureLabel;
    private Label humidityLabel;
    private Label soilMoistureLabel;
    private Circle lightsCircle;
    private Circle ventsCircle;
    private Circle waterCircle;
    private Circle heaterCircle;

    private AbsoluteLayout updateScreen;
    private Label cloudStatus;
    private Label progressValue;
    private ProgressBar progressBar;

    public DisplayController(IPixelDisplay _display, RotationType rotationType)
    {
        displayScreen = new DisplayScreen(_display, rotationType)
        {
            BackgroundColor = backgroundColor
        };

        LoadSplashScreen();

        LoadDataScreen();

        LoadOTAUpdateScreen();

        displayScreen.Controls.Add(splashScreen!, dataScreen!, updateScreen!);
    }

    private void LoadSplashScreen()
    {
        splashScreen = new AbsoluteLayout(displayScreen);

        var logo = Image.LoadFromResource("Cultivar.Assets.img_meadow.bmp");
        var displayImage = new Picture(
            0,
            displayScreen.Height / 4,
            displayScreen.Width,
            logo.Height,
            logo)
        {
            BackColor = backgroundColor,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        splashScreen.Controls.Add(displayImage);

        splashScreen.Controls.Add(new Label(
            0,
            170,
            displayScreen.Width,
            font8x12.Height)
        {
            Text = $"Cultivar v{MainController.VERSION}",
            TextColor = Color.White,
            Font = font8x12,
            ScaleFactor = ScaleFactor.X2,
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        splashScreen.IsVisible = false;
    }

    private void LoadDataScreen()
    {
        dataScreen = new AbsoluteLayout(displayScreen);

        LoadStatusBar();

        LoadCounter();

        LoadTemperatureIndicator();

        LoadHumidityIndicator();

        LoadSoilMoistureIndicator();

        LoadLightsStatus();

        LoadVentsStatus();

        LoadWaterStatus();

        LoadHeaterStatus();

        dataScreen.IsVisible = false;
    }
    private void LoadStatusBar()
    {
        int boxX = 0;
        int boxY = 0;
        int boxWidth = 320;
        int boxHeight = 30;

        statusLabel = new Label(boxX + 5, boxY, boxWidth, boxHeight)
        {
            Text = "-",
            Font = font12X20,
            TextColor = foregroundColor,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        dataScreen.Controls.Add(statusLabel);

        wifi = new Picture(286, boxY + 5, imgWifiFade.Width, imgWifiFade.Height, imgWifiFade);
        dataScreen.Controls.Add(wifi);

        cloud = new Picture(252, boxY + 5, imgCloudFade.Width, imgCloudFade.Height, imgCloudFade);
        dataScreen.Controls.Add(cloud);

        sync = new Picture(226, boxY + 5, imgSyncFade.Width, imgSyncFade.Height, imgSyncFade);
        dataScreen.Controls.Add(sync);
    }
    private void LoadCounter()
    {
        dataScreen.Controls.Add(new Box(160, 5, 60, 21)
        {
            ForeColor = Color.FromHex("082936")
        });
        counterLabel = new Label(160, 8, 60, 18)
        {
            Text = "000000",
            Font = font8x12,
            TextColor = foregroundColor,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        dataScreen.Controls.Add(counterLabel);
    }
    private void LoadTemperatureIndicator()
    {
        int boxX = 0;
        int boxY = 30;
        int boxWidth = 320;
        int boxHeight = 42;

        dataScreen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
        {
            ForeColor = temperatureColor
        });

        dataScreen.Controls.Add(new Label(boxX + 5, boxY, boxWidth - 10, boxHeight)
        {
            Text = "Temperature",
            Font = font16x24,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        });

        temperatureLabel = new Label(boxX + 5, boxY + 2, boxWidth - 10, boxHeight)
        {
            Text = "0",
            Font = font16x24,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };
        dataScreen.Controls.Add(temperatureLabel);
    }
    private void LoadHumidityIndicator()
    {
        int boxX = 0;
        int boxY = 72;
        int boxWidth = 320;
        int boxHeight = 42;

        dataScreen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
        {
            ForeColor = humidityColor
        });

        dataScreen.Controls.Add(new Label(boxX + 5, boxY, boxWidth - 10, boxHeight)
        {
            Text = "Humidity",
            Font = font16x24,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        });

        humidityLabel = new Label(boxX + 5, boxY + 2, boxWidth - 10, boxHeight)
        {
            Text = "0",
            Font = font16x24,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };
        dataScreen.Controls.Add(humidityLabel);
    }
    private void LoadSoilMoistureIndicator()
    {
        int boxX = 0;
        int boxY = 114;
        int boxWidth = 320;
        int boxHeight = 42;

        dataScreen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
        {
            ForeColor = soilMoistureColor
        });

        dataScreen.Controls.Add(new Label(boxX + 5, boxY, boxWidth - 10, boxHeight)
        {
            Text = "Soil Moisture",
            Font = font16x24,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        });

        soilMoistureLabel = new Label(boxX + 5, boxY + 2, boxWidth - 10, boxHeight)
        {
            Text = "0",
            Font = font16x24,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };
        dataScreen.Controls.Add(soilMoistureLabel);
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
        dataScreen.Controls.Add(lightsCircle);

        dataScreen.Controls.Add(new Circle(135, 173, 4)
        {
            ForeColor = Color.FromHex("DCDCDC"),
        });

        dataScreen.Controls.Add(new Label(boxX + 10, boxY + 2, boxWidth - 10, boxHeight)
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

        dataScreen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
        {
            ForeColor = Color.FromHex("082936")
        });

        ventsCircle = new Circle(299, 177, 12)
        {
            ForeColor = inactiveColor,
        };
        dataScreen.Controls.Add(ventsCircle);

        dataScreen.Controls.Add(new Circle(295, 173, 4)
        {
            ForeColor = Color.FromHex("DCDCDC"),
        });

        dataScreen.Controls.Add(new Label(boxX + 10, boxY + 2, boxWidth - 10, boxHeight)
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

        dataScreen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
        {
            ForeColor = Color.FromHex("082936")
        });

        waterCircle = new Circle(139, 219, 12)
        {
            ForeColor = inactiveColor,
        };
        dataScreen.Controls.Add(waterCircle);

        dataScreen.Controls.Add(new Circle(135, 215, 4)
        {
            ForeColor = Color.FromHex("DCDCDC")
        });

        dataScreen.Controls.Add(new Label(boxX + 10, boxY + 2, boxWidth - 10, boxHeight)
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
        dataScreen.Controls.Add(heaterCircle);

        dataScreen.Controls.Add(new Circle(295, 215, 4)
        {
            ForeColor = Color.FromHex("DCDCDC"),
        });

        dataScreen.Controls.Add(new Label(boxX + 10, boxY + 2, boxWidth - 10, boxHeight)
        {
            Text = "Heater",
            Font = font16x24,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        });
    }

    private void LoadOTAUpdateScreen()
    {
        updateScreen = new AbsoluteLayout(displayScreen);

        var logo = Image.LoadFromResource("Cultivar.Assets.img_meadow.bmp");
        var displayImage = new Picture(95, 33, logo.Width, logo.Height, logo)
        {
            BackColor = backgroundColor,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        updateScreen.Controls.Add(displayImage);

        updateScreen.Controls.Add(new Label(0, 135, updateScreen.Width, font16x24.Height)
        {
            Text = $"Cultivar v{MainController.VERSION:N1}",
            TextColor = Color.White,
            Font = font16x24,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        cloudStatus = new Label(0, 175, updateScreen.Width, font12x16.Height)
        {
            Text = "Updating...",
            TextColor = Color.White,
            Font = font12x16,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        updateScreen.Controls.Add(cloudStatus);

        progressBar = new ProgressBar(90, 205, 140, 16)
        {
            BackColor = Color.Black,
            ValueColor = Color.FromHex("0B3749"),
            BorderColor = Color.FromHex("0B3749"),
            IsVisible = true
        };
        updateScreen.Controls.Add(progressBar);

        progressValue = new Label(90, 206, 140, 16)
        {
            Text = "0%",
            TextColor = Color.White,
            Font = font12x16,
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = true
        };
        updateScreen.Controls.Add(progressValue);

        updateScreen.IsVisible = false;
    }

    public void UpdateDownloadProgress(int progress)
    {
        if (!progressBar.IsVisible)
        {
            progressBar.IsVisible = true;
            progressValue.IsVisible = true;
        }

        progressBar.Value = progress;
        progressValue.Text = $"{progress}%";

        if (progress == 100)
        {
            UpdateCloudStatus("Download Complete");
            Thread.Sleep(TimeSpan.FromSeconds(3));
            UpdateCloudStatus(string.Empty);
        }
    }
    public void UpdateCloudStatus(string status)
    {
        cloudStatus.Text = status;
    }

    public void ShowSplashScreen()
    {
        dataScreen.IsVisible = false;
        splashScreen.IsVisible = true;
    }
    public void ShowDataScreen()
    {
        splashScreen.IsVisible = false;
        dataScreen.IsVisible = true;
    }
    public void ShowUpdateScreen()
    {
        dataScreen.IsVisible = false;
        updateScreen.IsVisible = true;
    }

    public async Task StartConnectingWiFiAnimation()
    {
        connectivityToken = new CancellationTokenSource();

        bool alternateImg = false;

        while (!connectivityToken.IsCancellationRequested)
        {
            alternateImg = !alternateImg;
            UpdateConnectionStatus(alternateImg);

            await Task.Delay(500);
        }
    }
    public async Task StartConnectingCloudAnimation()
    {
        cloudToken = new CancellationTokenSource();

        bool alternateImg = false;

        while (!cloudToken.IsCancellationRequested)
        {
            alternateImg = !alternateImg;
            UpdateCloudStatus(alternateImg);

            await Task.Delay(500);
        }
    }

    public void UpdateConnectionStatus(bool connected, bool stopAnimation = false)
    {
        if (stopAnimation) { connectivityToken.Cancel(); }

        wifi.Image = connected ? imgWifi : imgWifiFade;
    }

    public void UpdateCloudStatus(bool IsConnected, bool stopAnimation = false)
    {
        if (stopAnimation) { cloudToken.Cancel(); }

        cloud.Image = IsConnected ? imgCloud : imgCloudFade;
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
        statusLabel.Text = status;
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
        displayScreen.BeginUpdate();

        counterLabel.Text = $"{logId:D6}";
        temperatureLabel.Text = $"{temp.ToString("N0")}°C";
        humidityLabel.Text = $"{humidity.ToString("N0")}%";
        soilMoistureLabel.Text = $"{moisture.ToString("N0")}%";

        displayScreen.EndUpdate();
    }
}