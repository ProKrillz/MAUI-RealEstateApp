using RealEstateApp.Models;
using System.Collections.ObjectModel;

namespace RealEstateApp.ViewModels
{
    public class HeightCalculatorPageViewModel :BaseViewModel
    {
        public ObservableCollection<BarometerMeasurement> BarometerMeasurementCollection { get; set; } = new();

        private double _currentPressure;
		public double CurrentPressure
        {
			get => _currentPressure; 
			set => SetProperty(ref _currentPressure, value); 
		}
    
        private double _currentAltitude;
        public double CurrentAltitude
        {
            get => _currentAltitude;
            set => SetProperty(ref _currentAltitude, value);
        }
        private string _measurementLabel;
        public string MeasurementLabel
        {
            get => _measurementLabel;
            set => SetProperty(ref _measurementLabel, value);
        }
        private Command _saveBarometerInfoCommand;
        public Command SaveBarometerInfoCommand => _saveBarometerInfoCommand ??= new Command(
            execute: () =>
            {
                if (BarometerMeasurementCollection.Any())
                {
                 
                    BarometerMeasurement last = BarometerMeasurementCollection.LastOrDefault();
                    BarometerMeasurementCollection.Add(new BarometerMeasurement
                    {
                        Label = MeasurementLabel,
                        Altitude = CurrentAltitude,
                        Pressure = CurrentPressure,
                        HeightChange = CurrentAltitude - last.Altitude,
                    });
                }
                else
                {
                  
                    BarometerMeasurementCollection.Add(new BarometerMeasurement
                    {
                        Label = MeasurementLabel,
                        Altitude = CurrentAltitude,
                        Pressure = CurrentPressure,
                    });
                }
            });
        public void ToggleBarometer()
        {
            Barometer.Default.ReadingChanged += Barometer_ReadingChanged;
            Barometer.Default.Start(SensorSpeed.UI);    
        }

        private void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            CurrentAltitude = 44307.694 * (1 - Math.Pow(e.Reading.PressureInHectopascals / 1006, 0.190284));
            CurrentPressure = e.Reading.PressureInHectopascals; //$"Barometer: {e.Reading.PressureInHectopascals} hPa";
        }
    }
}
