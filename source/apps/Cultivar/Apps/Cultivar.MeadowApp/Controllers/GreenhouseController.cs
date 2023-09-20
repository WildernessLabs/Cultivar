using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cultivar.MeadowApp.UI;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Logging;
using Meadow.Units;

namespace Cultivar.MeadowApp.Controllers
{
	public class GreenhouseController
	{
        // Singleton pattern (in case we want to change)
        //private static readonly Lazy<GreenhouseController> lazy = new Lazy<GreenhouseController>(() => new GreenhouseController());
        //public static GreenhouseController Current { get { return lazy.Value; } }
        //private GreenhouseController() { }
        //public Initialize(IGreenhouseHardware greenhouseHardware)

        protected IGreenhouseHardware Hardware { get; set; }

        protected DisplayController displayController;
        protected MicroAudio audio;
        CloudLogger cloudLogger;


        public GreenhouseController(IGreenhouseHardware greenhouseHardware)
        {
            this.Hardware = greenhouseHardware;

            cloudLogger = new CloudLogger(LogLevel.Warning);
            Resolver.Log.AddProvider(cloudLogger);
            Resolver.Services.Add(cloudLogger);

            Resolver.Log.Info($"cloudlogger null? {cloudLogger is null}");

            Hardware.RgbLed?.SetColor(Color.Blue);

            Hardware.Speaker?.SetVolume(0.5f);
            audio = new MicroAudio(Hardware.Speaker);

            //---- display controller (handles display updates)
            if (Hardware.Display is { } display)
            {
                Resolver.Log.Trace("Creating DisplayController");
                displayController = new DisplayController(display);
                Resolver.Log.Trace("DisplayController up");
            }

            //---- BH1750 Light Sensor
            if (Hardware.LightSensor is { } bh1750)
            {
                bh1750.Updated += Bh1750Updated;
            }

            //---- BME688 Atmospheric sensor
            if (Hardware.EnvironmentalSensor is { } bme688)
            {
                bme688.Updated += Bme688Updated;
            }

            //---- BMI270 Accel/IMU
            if (Hardware.MotionSensor is { } bmi270)
            {
                bmi270.Updated += Bmi270Updated;
            }

            //---- buttons
            if (Hardware.RightButton is { } rightButton)
            {
                rightButton.PressStarted += (s, e) => displayController.RightButtonState = true;
                rightButton.PressEnded += (s, e) => displayController.RightButtonState = false;
            }

            if (Hardware.DownButton is { } downButton)
            {
                downButton.PressStarted += (s, e) => displayController.DownButtonState = true;
                downButton.PressEnded += (s, e) => displayController.DownButtonState = false;
            }
            if (Hardware.LeftButton is { } leftButton)
            {
                leftButton.PressStarted += (s, e) => displayController.LeftButtonState = true;
                leftButton.PressEnded += (s, e) => displayController.LeftButtonState = false;
            }
            if (Hardware.UpButton is { } upButton)
            {
                upButton.PressStarted += (s, e) => displayController.UpButtonState = true;
                upButton.PressEnded += (s, e) => displayController.UpButtonState = false;
            }

            //---- heartbeat
            Resolver.Log.Info("Initialization complete");

        }

        public Task Run()
        {
            _ = audio.PlaySystemSound(SystemSoundEffect.Success);

            //---- BH1750 Light Sensor
            if (Hardware.LightSensor is { } bh1750)
            {
                bh1750.StartUpdating(TimeSpan.FromSeconds(5));
            }

            //---- BME688 Atmospheric sensor
            if (Hardware.EnvironmentalSensor is { } bme688)
            {
                bme688.StartUpdating(TimeSpan.FromSeconds(5));
            }

            //---- BMI270 Accel/IMU
            if (Hardware.MotionSensor is { } bmi270)
            {
                bmi270.StartUpdating(TimeSpan.FromSeconds(5));
            }

            displayController?.Update();

            Resolver.Log.Info("starting blink");
            _ = Hardware.RgbLed?.StartBlink(WildernessLabsColors.PearGreen, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(2000), 0.5f);


            return Task.CompletedTask;
        }


        private void Bmi270Updated(object sender, IChangeResult<(Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature)> e)
        {
            Resolver.Log.Info($"BMI270: {e.New.Acceleration3D.Value.X.Gravity:0.0},{e.New.Acceleration3D.Value.Y.Gravity:0.0},{e.New.Acceleration3D.Value.Z.Gravity:0.0}g");
            if (displayController != null)
            {
                displayController.AccelerationConditions = e.New;
            }
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688: {(int)e.New.Temperature?.Celsius}C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");
            if (displayController != null)
            {
                displayController.AtmosphericConditions = e.New;
            }
            Resolver.Log.Info($"Logging to cloud.");
            try
            {
                var cl = Resolver.Services.Get<CloudLogger>();
                cl.LogEvent(110, "Atmospheric reading", new Dictionary<string, object>()
                {
                    { "TemperatureCelsius", e.New.Temperature?.Celsius },
                    { "HumidityPercentage", e.New.Humidity?.Percent },
                    { "PressureMillibar", e.New.Pressure?.Millibar }
                });
            }
            catch (Exception ex) {
                Resolver.Log.Info($"Err: {ex.Message}");
            }
        }

        private void Bh1750Updated(object sender, IChangeResult<Illuminance> e)
        {
            Resolver.Log.Info($"BH1750: {e.New.Lux}");
            if (displayController != null)
            {
                displayController.LightConditions = e.New;
            }
        }

    }
}