using OxyPlot;
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

        public ObservableCollection<DataPoint> TemperatureLogs { get; set; }

        public ObservableCollection<DataPoint> HumidityLogs { get; set; }

        public ObservableCollection<DataPoint> SoilMoistureLogs { get; set; }

        public MainWindowViewModel()
        {
            ToggleLightsCommand = ReactiveCommand.Create(ToggleLights);

            ToggleHeaterCommand = ReactiveCommand.Create(ToggleHeater);

            ToggleVentilationCommand = ReactiveCommand.Create(ToggleVentilation);

            ToggleSprinklerCommand = ReactiveCommand.Create(ToggleSprinkler);

            TemperatureLogs = new ObservableCollection<DataPoint>();

            HumidityLogs = new ObservableCollection<DataPoint>();

            SoilMoistureLogs = new ObservableCollection<DataPoint>();

            //for (; _sampleNumber < 50;)
            //{
            //    Add();
            //}

            _ = SimulateCurrentConditions();
        }


        private int _sampleNumber = 50;
        private void Add()
        {
            TemperatureLogs.Add(new DataPoint(_sampleNumber, (Math.Sin(_sampleNumber / 4.0) * 2) + 1.5));
            HumidityLogs.Add(new DataPoint(_sampleNumber, Math.Cos(_sampleNumber / 3.0)));
            SoilMoistureLogs.Add(new DataPoint(_sampleNumber, Math.Cos(_sampleNumber / 6.0) * 50));

            _sampleNumber++;

            // just an example of a moving window
            while (TemperatureLogs.Count > 55)
            {
                TemperatureLogs.RemoveAt(0);
                HumidityLogs.RemoveAt(0);
                SoilMoistureLogs.RemoveAt(0);
            }
        }

        async Task SimulateCurrentConditions()
        {
            var random = new Random();

            int i = 0;

            while (true)
            {
                double temp = random.Next(23, 28) + random.NextDouble();
                int h = random.Next(92, 97);
                double sm = random.Next(75, 77) + random.NextDouble();

                CurrentTemperature = $"{temp:N0}°C";
                CurrentHumidity = $"{h}%";
                CurrentSoilMoisture = $"{sm:N0}%";

                TemperatureLogs.Add(new DataPoint(i, temp));
                HumidityLogs.Add(new DataPoint(i, h));
                SoilMoistureLogs.Add(new DataPoint(i, sm));

                await Task.Delay(TimeSpan.FromSeconds(5));

                if (i > 50)
                {
                    TemperatureLogs.RemoveAt(0);
                    HumidityLogs.RemoveAt(0);
                    SoilMoistureLogs.RemoveAt(0);
                }

                i++;
            }
        }

        public void ToggleLights()
        {
            IsLightsOn = !IsLightsOn;
            Add();
        }

        public void ToggleHeater()
        {
            IsHeaterOn = !IsHeaterOn;
        }

        public void ToggleVentilation()
        {
            IsVentilationOn = !IsVentilationOn;
        }

        public void ToggleSprinkler()
        {
            IsSprinklerOn = !IsSprinklerOn;
        }
    }
}