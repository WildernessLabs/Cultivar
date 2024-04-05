using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;

namespace Cultivar.MeadowApp.Controllers;

public class DisplayController
{
    private readonly Image imgWifi = Image.LoadFromResource("Cultivar.Assets.img-wifi.bmp");
    private readonly Image imgSync = Image.LoadFromResource("Cultivar.Assets.img-sync.bmp");
    private readonly Image imgWifiFade = Image.LoadFromResource("Cultivar.Assets.img-wifi-fade.bmp");
    private readonly Image imgSyncFade = Image.LoadFromResource("Cultivar.Assets.img-sync-fade.bmp");
    private readonly Image imgRed = Image.LoadFromResource("Cultivar.Assets.img-red.bmp");
    private readonly Image imgGreen = Image.LoadFromResource("Cultivar.Assets.img-green.bmp");

    private readonly Color backgroundColor = Color.White;
    private readonly Color foregroundColor = Color.Black;
    private readonly Color temperatureColor = Color.FromHex("B35E2C");
    private readonly Color humidityColor = Color.FromHex("1A80AA");
    private readonly Color soilMoistureColor = Color.FromHex("98A645");
    private readonly Color sensorColor = Color.White;

    private readonly Font12x20 font12X20 = new Font12x20();
    private readonly Font12x16 font12X16 = new Font12x16();
    private readonly Font8x12 font8x12 = new Font8x12();

    private readonly DisplayScreen screen;

    private Label StatusLabel;
    private Label CounterLabel;
    private Label TemperatureLabel;
    private Label HumidityLabel;
    private Label SoilMoistureLabel;
    private Picture ledLights;
    private Picture wifi;
    private Picture sync;
    private Picture ledVents;
    private Picture ledWater;
    private Picture ledHeater;

    private int counter = 0;

    public DisplayController(IPixelDisplay _display, RotationType rotation)
    {
        screen = new DisplayScreen(_display, rotation)
        {
            BackgroundColor = backgroundColor
        };

        screen.Controls.Add(new Box(0, 27, 106, 93) { ForeColor = temperatureColor });
        screen.Controls.Add(new Box(106, 27, 108, 93) { ForeColor = humidityColor });
        screen.Controls.Add(new Box(214, 27, 106, 93) { ForeColor = soilMoistureColor });

        screen.Controls.Add(new Box(160, 120, 1, screen.Height) { ForeColor = foregroundColor, IsFilled = false });
        screen.Controls.Add(new Box(0, 180, screen.Width, 1) { ForeColor = foregroundColor, IsFilled = false });

        StatusLabel = new Label(2, 6, 12, 16)
        {
            Text = "-",
            Font = font12X20,
            TextColor = foregroundColor,
        };
        screen.Controls.Add(StatusLabel);

        wifi = new Picture(286, 3, 30, 21, imgWifiFade);
        screen.Controls.Add(wifi);

        sync = new Picture(260, 3, 21, 21, imgSyncFade);
        screen.Controls.Add(sync);

        screen.Controls.Add(new Box(195, 3, 60, 21)
        {
            ForeColor = Color.FromHex("333333")
        });
        CounterLabel = new Label(195, 6, 60, 18)
        {
            Text = "000010",
            Font = font8x12,
            TextColor = Color.White,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        screen.Controls.Add(CounterLabel);

        screen.Controls.Add(new Label(5, 32, 12, 16)
        {
            Text = "Temp",
            Font = font12X16,
            TextColor = sensorColor
        });
        screen.Controls.Add(new Label(77, 99, 12, 16)
        {
            Text = "°C",
            Font = font12X20,
            TextColor = sensorColor
        });

        screen.Controls.Add(new Label(111, 32, 12, 16)
        {
            Text = "Humidity",
            Font = font12X16,
            TextColor = sensorColor
        });
        screen.Controls.Add(new Label(197, 99, 12, 16)
        {
            Text = "%",
            Font = font12X20,
            TextColor = sensorColor
        });

        screen.Controls.Add(new Label(219, 32, 12, 16)
        {
            Text = "Soil",
            Font = font12X16,
            TextColor = sensorColor
        });
        screen.Controls.Add(new Label(303, 99, 12, 16)
        {
            Text = "%",
            Font = font12X20,
            TextColor = sensorColor
        });

        TemperatureLabel = new Label(50, 70, 12, 16, ScaleFactor.X2)
        {
            Text = "0",
            Font = font12X16,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        screen.Controls.Add(TemperatureLabel);

        HumidityLabel = new Label(155, 70, 12, 16, ScaleFactor.X2)
        {
            Text = "0",
            Font = font12X16,
            TextColor = sensorColor,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        screen.Controls.Add(HumidityLabel);

        SoilMoistureLabel = new Label(260, 70, 12, 16, ScaleFactor.X2)
        {
            Text = "0",
            Font = font12X16,
            TextColor = sensorColor,
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
            Font = font8x12,
            TextColor = foregroundColor
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
            Font = font8x12,
            TextColor = foregroundColor
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
            Font = font8x12,
            TextColor = foregroundColor
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
            Font = font8x12,
            TextColor = foregroundColor
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

        counter++;
        CounterLabel.Text = $"{counter:D6}";

        TemperatureLabel.Text = temp.ToString("N0");
        HumidityLabel.Text = humidity.ToString("N0");
        SoilMoistureLabel.Text = moisture.ToString("N0");

        screen.EndUpdate();
    }
}