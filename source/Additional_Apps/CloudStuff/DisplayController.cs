using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using Illuminance = Meadow.Units.Illuminance;
using Temperature = Meadow.Units.Temperature;

namespace MeadowApp
{
    public class DisplayController
    {
        readonly MicroGraphics graphics;

        public Temperature? TemperatureConditions
        {
            get => temperatureConditions;
            set
            {
                temperatureConditions = value;
                Update();
            }
        }
        Temperature? temperatureConditions;

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

        public (Meadow.Units.Acceleration3D? Acceleration3D, Meadow.Units.AngularVelocity3D? AngularVelocity3D, Temperature? Temperature) AccelerationConditions
        {
            get => accelerationConditions;
            set
            {
                accelerationConditions = value;
                Update();
            }
        }
        (Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature) accelerationConditions;


        public bool UpButtonState
        {
            get => upButtonState;
            set
            {
                upButtonState = value;
                Update();
            }
        }
        bool upButtonState = false;

        public bool DownButtonState
        {
            get => downButtonState;
            set
            {
                downButtonState = value;
                Update();
            }
        }
        bool downButtonState = false;

        public bool LeftButtonState
        {
            get => leftButtonState;
            set
            {
                leftButtonState = value;
                Update();
            }
        }
        bool leftButtonState = false;

        public bool RightButtonState
        {
            get => rightButtonState;
            set
            {
                rightButtonState = value;
                Update();
            }
        }
        bool rightButtonState = false;

        bool isUpdating = false;
        bool needsUpdate = false;

        public DisplayController(IPixelDisplay display)
        {
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
            graphics.DrawText(x: 2, y: 0, "Hello PROJ LAB!", WildernessLabsColors.AzureBlue);

            if (TemperatureConditions is { } temperature)
            {
                DrawStatus("Temperature:", $"{temperature.Celsius:N1}C", WildernessLabsColors.GalleryWhite, 35);
            }

            if (LightConditions is { } light)
            {
                DrawStatus("Lux:", $"{light:N0}Lux", WildernessLabsColors.GalleryWhite, 95);
            }

            if (AccelerationConditions is { } acceleration)
            {
                if (acceleration.Acceleration3D is { } accel3D)
                {
                    DrawStatus("Accel:", $"{accel3D.X.Gravity:0.#},{accel3D.Y.Gravity:0.#},{accel3D.Z.Gravity:0.#}g", WildernessLabsColors.AzureBlue, 115);
                }

                if (acceleration.AngularVelocity3D is { } angular3D)
                {
                    DrawStatus("Gyro:", $"{angular3D.X:0},{angular3D.Y:0},{angular3D.Z:0}rpm", WildernessLabsColors.AzureBlue, 135);
                }
            }

            DrawStatus("Left:", $"{(LeftButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire, 200);
            DrawStatus("Down:", $"{(DownButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire, 180);
            DrawStatus("Up:", $"{(UpButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire, 160);
            DrawStatus("Right:", $"{(RightButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire, 220);
        }
    }
}