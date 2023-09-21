using Cultivar_reTerminal.Client;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace Cultivar_reTerminal.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isLightsOn;
        public bool IsLightsOn
        {
            get => _isLightsOn;
            set => this.RaiseAndSetIfChanged(ref _isLightsOn, value);
        }

        private bool _isHeaterOn;
        public bool IsHeaterOn
        {
            get => _isHeaterOn;
            set => this.RaiseAndSetIfChanged(ref _isHeaterOn, value);
        }

        private bool _isVentilationOn;
        public bool IsVentilationOn
        {
            get => _isVentilationOn;
            set => this.RaiseAndSetIfChanged(ref _isVentilationOn, value);
        }

        private bool _isSprinklerOn;
        public bool IsSprinklerOn
        {
            get => _isSprinklerOn;
            set => this.RaiseAndSetIfChanged(ref _isSprinklerOn, value);
        }

        private string _currentTemperature;
        public string CurrentTemperature
        {
            get => _currentTemperature;
            set => this.RaiseAndSetIfChanged(ref _currentTemperature, value);
        }

        private string _currentHumidity;
        public string CurrentHumidity
        {
            get => _currentHumidity;
            set => this.RaiseAndSetIfChanged(ref _currentHumidity, value);
        }

        private string _currentSoilMoisture;
        public string CurrentSoilMoisture
        {
            get => _currentSoilMoisture;
            set => this.RaiseAndSetIfChanged(ref _currentSoilMoisture, value);
        }

        public ReactiveCommand<Unit, Unit> ToggleLightsCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleHeaterCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleVentilationCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleSprinklerCommand { get; set; }

        public ObservableCollection<Pnl> TemperatureLogs { get; set; }

        public ObservableCollection<Pnl> HumidityLogs { get; set; }

        public ObservableCollection<Pnl> SoilMoistureLogs { get; set; }

        public MainWindowViewModel()
        {
            ToggleLightsCommand = ReactiveCommand.Create(ToggleLights);

            ToggleHeaterCommand = ReactiveCommand.Create(ToggleHeater);

            ToggleVentilationCommand = ReactiveCommand.Create(ToggleVentilation);

            ToggleSprinklerCommand = ReactiveCommand.Create(ToggleSprinkler);

            TemperatureLogs = new ObservableCollection<Pnl>();

            HumidityLogs = new ObservableCollection<Pnl>();

            SoilMoistureLogs = new ObservableCollection<Pnl>();

            //_ = SimulateCurrentConditions();
            _ = GetGreenhouseData();
        }

        async Task GetGreenhouseData()
        {
            var sensorReadings = await RestClient.GetSensorReadings();

            if (sensorReadings != null && sensorReadings.Count > 0)
            {
                foreach (var reading in sensorReadings)
                {
                    TemperatureLogs.Add(new Pnl(reading.record.timestamp, reading.record.measurements.TemperatureCelsius));
                    HumidityLogs.Add(new Pnl(reading.record.timestamp, reading.record.measurements.HumidityPercentage));
                    SoilMoistureLogs.Add(new Pnl(reading.record.timestamp, reading.record.measurements.HumidityPercentage - 10));
                }

                CurrentTemperature = $"{TemperatureLogs[0].Value:N0}°C";
                CurrentHumidity = $"{HumidityLogs[0].Value:N0}%";
                CurrentSoilMoisture = $"{SoilMoistureLogs[0].Value:N0}%";
            }
        }

        async Task SimulateCurrentConditions()
        {
            var random = new Random();

            while (true)
            {
                var dateTime = DateTime.Now;

                double temp = random.Next(26, 28) + random.NextDouble();
                int h = random.Next(95, 97);
                double sm = random.Next(75, 77) + random.NextDouble();

                CurrentTemperature = $"{temp:N0}°C";
                CurrentHumidity = $"{h}%";
                CurrentSoilMoisture = $"{sm:N0}%";

                TemperatureLogs.Add(new Pnl(dateTime, temp));
                HumidityLogs.Add(new Pnl(dateTime, h));
                SoilMoistureLogs.Add(new Pnl(dateTime, sm));

                await Task.Delay(TimeSpan.FromMinutes(1));

                if (TemperatureLogs.Count > 10)
                {
                    TemperatureLogs.RemoveAt(0);
                    HumidityLogs.RemoveAt(0);
                    SoilMoistureLogs.RemoveAt(0);
                }
            }
        }

        public async void ToggleLights()
        {
            var s = await RestClient.GetSensorReadings();

            //var res = await RestClient.SendCommand(CultivarCommands.LightControl, !IsLightsOn);
            //if (res)
            //{
            //    IsLightsOn = !IsLightsOn;
            //}
        }

        public async void ToggleHeater()
        {
            var res = await RestClient.SendCommand(CultivarCommands.HeaterControl, !IsHeaterOn);
            if (res)
            {
                IsHeaterOn = !IsHeaterOn;
            }
        }

        public async void ToggleVentilation()
        {
            var res = await RestClient.SendCommand(CultivarCommands.FanControl, !IsVentilationOn);
            if (res)
            {
                IsVentilationOn = !IsVentilationOn;
            }
        }

        public async void ToggleSprinkler()
        {
            var res = await RestClient.SendCommand(CultivarCommands.ValveControl, !IsSprinklerOn);
            if (res)
            {
                IsSprinklerOn = !IsSprinklerOn;
            }
        }
    }

    public class Pnl
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }

        public Pnl(DateTime time, double value)
        {
            Time = time;
            Value = value;
        }

        public override string ToString()
        {
            return String.Format("{0:HH:mm} {1:0.0}", this.Time, this.Value);
        }
    }
}