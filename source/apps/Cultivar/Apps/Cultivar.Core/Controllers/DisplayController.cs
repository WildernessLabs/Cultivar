using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;

namespace Cultivar.MeadowApp.Controllers;

public class DisplayController
{
    private readonly Image imgWifi = Image.LoadFromResource("Cultivar.Assets.img-wifi.bmp");
    private readonly Image imgSync = Image.LoadFromResource("Cultivar.Assets.img-sync.bmp");
    private readonly Image imgCloud = Image.LoadFromResource("Cultivar.Assets.img-cloud.bmp");
    private readonly Image imgWifiFade = Image.LoadFromResource("Cultivar.Assets.img-wifi-fade.bmp");
    private readonly Image imgSyncFade = Image.LoadFromResource("Cultivar.Assets.img-sync-fade.bmp");
    private readonly Image imgCloudFade = Image.LoadFromResource("Cultivar.Assets.img-cloud-fade.bmp");

    private readonly Color backgroundColor = Color.White;
    private readonly Color foregroundColor = Color.Black;
    private readonly Color temperatureColor = Color.FromHex("B35E2C");
    private readonly Color humidityColor = Color.FromHex("1A80AA");
    private readonly Color soilMoistureColor = Color.FromHex("98A645");
    private readonly Color sensorColor = Color.White;
    private readonly Color activeColor = Color.FromHex("14B069");
    private readonly Color inactiveColor = Color.FromHex("FF3535");

    private readonly Font12x20 font12X20 = new Font12x20();
    private readonly Font12x16 font12X16 = new Font12x16();
    private readonly Font8x12 font8x12 = new Font8x12();
    private readonly Font6x8 font6x8 = new Font6x8();

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

    private int counter = 0;

    public DisplayController(IPixelDisplay _display, RotationType rotation)
    {
        screen = new DisplayScreen(_display, rotation)
        {
            BackgroundColor = backgroundColor
        };

        screen.Controls.Add(new Box(160, 120, 1, screen.Height) { ForeColor = foregroundColor, IsFilled = false });
        screen.Controls.Add(new Box(0, 180, screen.Width, 1) { ForeColor = foregroundColor, IsFilled = false });

        StatusLabel = new Label(2, 6, 12, 16)
        {
            Text = "Connected",
            Font = font12X20,
            TextColor = foregroundColor,
        };
        screen.Controls.Add(StatusLabel);

        wifi = new Picture(286, 3, 30, 21, imgWifiFade);
        screen.Controls.Add(wifi);

        cloud = new Picture(252, 3, 30, 21, imgCloudFade);
        screen.Controls.Add(cloud);

        sync = new Picture(226, 3, 21, 21, imgSyncFade);
        screen.Controls.Add(sync);

        LoadCounter();

        LoadTemperatureIndicator();

        LoadHumidityIndicator();

        LoadSoilMoistureIndicator();

        LoadLightsStatus();

        LoadVentsStatus();

        LoadWaterStatus();

        LoadHeaterStatus();
    }

    private void LoadCounter()
    {
        screen.Controls.Add(new Box(160, 3, 60, 21)
        {
            ForeColor = Color.FromHex("333333")
        });
        CounterLabel = new Label(160, 6, 60, 18)
        {
            Text = "000000",
            Font = font8x12,
            TextColor = Color.White,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        screen.Controls.Add(CounterLabel);
    }

    private void LoadTemperatureIndicator()
    {
        int boxX = 0;
        int boxY = 27;
        int boxWidth = 106;
        int boxHeight = 93;

        screen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
        {
            ForeColor = temperatureColor
        });

        screen.Controls.Add(new Label(5, boxY + 5, boxWidth - 10, font8x12.Height)
        {
            Text = "Temperature",
            Font = font8x12,
            TextColor = sensorColor
        });

        TemperatureLabel = new Label(5, boxY + 43, boxWidth - 10, font12X16.Height, ScaleFactor.X2)
        {
            Text = "0",
            Font = font12X16,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        screen.Controls.Add(TemperatureLabel);

        screen.Controls.Add(new Label(5, boxY + 72, boxWidth - 10, font12X20.Height)
        {
            Text = "°C",
            Font = font12X20,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Right,
        });
    }

    private void LoadHumidityIndicator()
    {
        int boxX = 106;
        int boxY = 27;
        int boxWidth = 108;
        int boxHeight = 93;

        screen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
        {
            ForeColor = humidityColor
        });

        screen.Controls.Add(new Label(111, boxY + 5, boxWidth - 10, font8x12.Height)
        {
            Text = "Humidity",
            Font = font8x12,
            TextColor = sensorColor
        });

        HumidityLabel = new Label(111, boxY + 43, boxWidth - 10, font12X16.Height, ScaleFactor.X2)
        {
            Text = "0",
            Font = font12X16,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        screen.Controls.Add(HumidityLabel);

        screen.Controls.Add(new Label(111, boxY + 72, boxWidth - 10, font12X20.Height)
        {
            Text = "%",
            Font = font12X20,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Right
        });
    }

    private void LoadSoilMoistureIndicator()
    {
        int boxX = 214;
        int boxY = 27;
        int boxWidth = 106;
        int boxHeight = 93;

        screen.Controls.Add(new Box(boxX, boxY, boxWidth, boxHeight)
        {
            ForeColor = soilMoistureColor
        });

        screen.Controls.Add(new Label(219, boxY + 5, boxWidth - 10, font8x12.Height)
        {
            Text = "Soil Moist.",
            Font = font8x12,
            TextColor = sensorColor
        });

        SoilMoistureLabel = new Label(219, boxY + 43, boxWidth - 10, font12X16.Height, ScaleFactor.X2)
        {
            Text = "0",
            Font = font12X16,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        screen.Controls.Add(SoilMoistureLabel);

        screen.Controls.Add(new Label(219, boxY + 72, boxWidth - 10, font12X20.Height)
        {
            Text = "%",
            Font = font12X20,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Right
        });
    }

    private void LoadLightsStatus()
    {
        screen.Controls.Add(new Box(0, 120, screen.Width / 2, 60)
        {
            ForeColor = Color.FromHex("CCCCCC"),
        });

        lightsCircle = new Circle(30, 150, 20)
        {
            ForeColor = inactiveColor,
        };
        screen.Controls.Add(lightsCircle);

        screen.Controls.Add(new Circle(24, 144, 6)
        {
            ForeColor = Color.FromHex("DCDCDC"),
        });

        screen.Controls.Add(new Label(60, 145, 12, 16, ScaleFactor.X2)
        {
            Text = "Lights",
            Font = font8x12,
            TextColor = foregroundColor
        });
    }

    private void LoadVentsStatus()
    {
        screen.Controls.Add(new Box(screen.Width / 2, 120, screen.Width / 2, 60)
        {
            ForeColor = Color.White,
        });

        ventsCircle = new Circle(190, 150, 20)
        {
            ForeColor = inactiveColor,
        };
        screen.Controls.Add(ventsCircle);

        screen.Controls.Add(new Circle(184, 144, 6)
        {
            ForeColor = Color.FromHex("DCDCDC"),
        });

        screen.Controls.Add(new Label(220, 145, 12, 16, ScaleFactor.X2)
        {
            Text = "Vents",
            Font = font8x12,
            TextColor = foregroundColor
        });
    }

    private void LoadWaterStatus()
    {
        screen.Controls.Add(new Box(0, 180, screen.Width / 2, 60)
        {
            ForeColor = Color.White,
        });

        waterCircle = new Circle(30, 210, 20)
        {
            ForeColor = inactiveColor,
        };
        screen.Controls.Add(waterCircle);

        screen.Controls.Add(new Circle(24, 204, 6)
        {
            ForeColor = Color.FromHex("DCDCDC"),
        });

        screen.Controls.Add(new Label(60, 205, 12, 16, ScaleFactor.X2)
        {
            Text = "Water",
            Font = font8x12,
            TextColor = foregroundColor
        });
    }

    private void LoadHeaterStatus()
    {
        screen.Controls.Add(new Box(screen.Width / 2, 180, screen.Width / 2, 60)
        {
            ForeColor = Color.FromHex("CCCCCC"),
        });

        heaterCircle = new Circle(190, 210, 20)
        {
            ForeColor = inactiveColor,
        };
        screen.Controls.Add(heaterCircle);

        screen.Controls.Add(new Circle(184, 204, 6)
        {
            ForeColor = Color.FromHex("DCDCDC"),
        });

        screen.Controls.Add(new Label(220, 205, 12, 16, ScaleFactor.X2)
        {
            Text = "Heater",
            Font = font8x12,
            TextColor = foregroundColor
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

    public void UpdateReadings(double temp, double humidity, double moisture)
    {
        screen.BeginUpdate();

        counter++;
        CounterLabel.Text = $"{counter:D6}";

        TemperatureLabel.Text = temp.ToString("N0");
        HumidityLabel.Text = humidity.ToString("N0");
        SoilMoistureLabel.Text = moisture.ToString("N0");

        screen.EndUpdate();
    }
}