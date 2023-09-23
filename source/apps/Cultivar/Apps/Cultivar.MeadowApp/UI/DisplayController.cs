using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace Cultivar.MeadowApp.UI
{
    public class DisplayController
    {
        readonly MicroGraphics graphics;

        public (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)? AtmosphericConditions
        {
            get => atmosphericConditions;
            set
            {
                atmosphericConditions = value;
                Update();
            }
        }
        (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)? atmosphericConditions;

        public Illuminance? LightConditions
        {
            get => lightConditions;
            set
            {
                lightConditions = value;
                Update();
            }
        }
        Illuminance? lightConditions;

        public (Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature) AccelerationConditions
        {
            get => accelerationConditions;
            set
            {
                accelerationConditions = value;
                Update();
            }
        }
        (Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature) accelerationConditions;

        public bool FanState {
            get => _fanState;
            set
            {
                _fanState = value;
                Update();
            }
        }
        bool _fanState = false;

        public bool IrrigationState {
            get => _irrigationState;
            set
            {
                _irrigationState = value;
                Update();
            }
        }
        bool _irrigationState = false;

        public bool LightsState {
            get => _lightsState;
            set {
                _lightsState = value;
                Update();
            }
        }
        bool _lightsState = false;

        public bool HeaterState {
            get => _heaterState;
            set {
                _heaterState = value;
                Update();
            }
        }
        bool _heaterState = false;

        public bool WiFiConnected
        {
            get => wifiConnected;
            set {
                wifiConnected = value;
                Update();
            }
        } bool wifiConnected = false;

        bool isUpdating = false;
        bool needsUpdate = false;

        public DisplayController(IGraphicsDisplay display) {
            graphics = new MicroGraphics(display)
            {
                CurrentFont = new Font12x16()
            };

            graphics.Clear(true);
        }

        public void Update()
        {
            if (isUpdating)
            {   //queue up the next update
                needsUpdate = true;
                return;
            }

            isUpdating = true;

            graphics.Clear();
            Draw();
            graphics.Show();

            isUpdating = false;

            if (needsUpdate)
            {
                needsUpdate = false;
                Update();
            }
        }

        void DrawStatus(string label, string value, Color color, int yPosition)
        {
            graphics.DrawText(x: 2, y: yPosition, label, color: color);
            graphics.DrawText(x: graphics.Width - 2, y: yPosition, value, alignmentH: HorizontalAlignment.Right, color: color);
        }

        void Draw()
        {
            // title
            graphics.DrawText(x: 2, y: 0, "Cultivar", WildernessLabsColors.PearGreen);

            // wifi
            graphics.DrawText(x: 2, y: 20, $"Wifi {(WiFiConnected ? "" : "Not " )}Connected", WildernessLabsColors.AzureBlue);

            // Atmospheric conditions
            if (AtmosphericConditions is { } conditions) {
                if (conditions.Temperature is { } temp) {
                    DrawStatus("Temperature:", $"{temp.Celsius:N1}C", WildernessLabsColors.GalleryWhite, 35);
                }
                if (conditions.Pressure is { } pressure) {
                    DrawStatus("Pressure:", $"{pressure.StandardAtmosphere:N1}atm", WildernessLabsColors.GalleryWhite, 55);
                }
                if (conditions.Humidity is { } humidity) {
                    DrawStatus("Humidity:", $"{humidity.Percent:N1}%", WildernessLabsColors.GalleryWhite, 75);
                }
            }

            // light
            if (LightConditions is { } light) {
                DrawStatus("Lux:", $"{light:N0}Lux", WildernessLabsColors.GalleryWhite, 95);
            }

            // accel
            if (AccelerationConditions is { } acceleration) {
                if (acceleration.Acceleration3D is { } accel3D) {
                    DrawStatus("Accel:", $"{accel3D.X.Gravity:0.#},{accel3D.Y.Gravity:0.#},{accel3D.Z.Gravity:0.#}g", WildernessLabsColors.AzureBlue, 115);
                }
                if (acceleration.AngularVelocity3D is { } angular3D) {
                    DrawStatus("Gyro:", $"{angular3D.X:0},{angular3D.Y:0},{angular3D.Z:0}rpm", WildernessLabsColors.AzureBlue, 135);
                }
            }

            DrawStatus("Lights:", $"{(LightsState ? "On" : "Off")}", WildernessLabsColors.ChileanFire, 200);
            DrawStatus("Irrigation:", $"{(IrrigationState ? "On" : "Off")}", WildernessLabsColors.ChileanFire, 180);
            DrawStatus("Vent Fan:", $"{(FanState ? "On" : "Off")}", WildernessLabsColors.ChileanFire, 160);
            DrawStatus("Heater:", $"{(HeaterState ? "On" : "Off")}", WildernessLabsColors.ChileanFire, 220);
        }
    }
}