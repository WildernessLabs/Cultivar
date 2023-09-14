using OxyPlot;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace Cultivar_reTerminal.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isTemperatureChartVisible;
        public bool IsTemperatureChartVisible
        {
            get => _isTemperatureChartVisible;
            set
            {
                if (value)
                {
                    IsHumidityChartVisible = IsSoilMoistureChartVisible = false;
                }
                this.RaiseAndSetIfChanged(ref _isTemperatureChartVisible, value);
            }
        }

        private bool _isHumidityChartVisible;
        public bool IsHumidityChartVisible
        {
            get => _isHumidityChartVisible;
            set
            {
                if (value)
                {
                    IsTemperatureChartVisible = IsSoilMoistureChartVisible = false;
                }
                this.RaiseAndSetIfChanged(ref _isHumidityChartVisible, value);
            }
        }

        private bool _isSoilMoistureChartVisible;
        public bool IsSoilMoistureChartVisible
        {
            get => _isSoilMoistureChartVisible;
            set
            {
                if (value)
                {
                    IsTemperatureChartVisible = IsHumidityChartVisible = false;
                }
                this.RaiseAndSetIfChanged(ref _isSoilMoistureChartVisible, value);
            }
        }

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

        public ReactiveCommand<Unit, Unit> ViewTemperatureChartCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ViewHumidityChartCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ViewSoilMoistureChartCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleLightsCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleHeaterCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleVentilationCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ToggleSprinklerCommand { get; set; }

        public IList<DataPoint> Series1 { get; set; }
        public IList<DataPoint> Series2 { get; set; }
        public IList<DataPoint> Series3 { get; set; }

        public MainWindowViewModel()
        {
            IsTemperatureChartVisible = true;

            ViewTemperatureChartCommand = ReactiveCommand.Create(() =>
            {
                IsTemperatureChartVisible = true;
            });

            ViewHumidityChartCommand = ReactiveCommand.Create(() =>
            {
                IsHumidityChartVisible = true;
            });

            ViewSoilMoistureChartCommand = ReactiveCommand.Create(() =>
            {
                IsSoilMoistureChartVisible = true;
            });

            ToggleLightsCommand = ReactiveCommand.Create(ToggleLights);

            ToggleHeaterCommand = ReactiveCommand.Create(ToggleHeater);

            ToggleVentilationCommand = ReactiveCommand.Create(ToggleVentilation);

            ToggleSprinklerCommand = ReactiveCommand.Create(ToggleSprinkler);

            Series1 = new List<DataPoint>();
            Series2 = new List<DataPoint>();
            Series3 = new List<DataPoint>();

            for (var i = 0; i < 50; i++)
            {
                Series1.Add(new DataPoint(i, (Math.Sin(i / 4.0) * 2) + 1.5));
                Series2.Add(new DataPoint(i, Math.Cos(i / 3.0)));
                Series3.Add(new DataPoint(i, Math.Cos(i / 6.0) * 50));
            }
        }

        public void ToggleLights()
        {
            IsLightsOn = !IsLightsOn;
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