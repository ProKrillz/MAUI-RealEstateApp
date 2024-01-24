using RealEstateApp.Models;

namespace RealEstateApp.ViewModels
{
    [QueryProperty(nameof(Property), "MyProperty")]

    public class CompassViewModel : BaseViewModel
    {
        Property property;
        public Property Property 
        { 
            get => property; 
            set => SetProperty(ref property, value); 
        }

        private string _currentHeading;
        public string CurrentHeading
        {
            get => _currentHeading;
            set => SetProperty(ref _currentHeading, value); 
        }

        public void ToggleCompass()
        {
            if (Compass.Default.IsSupported)
            {
                if (!Compass.Default.IsMonitoring)
                {
                    // Turn on compass
                    Compass.Default.ReadingChanged += Compass_ReadingChanged;
                    Compass.Default.Start(SensorSpeed.UI);
                }
                else
                {
                    // Turn off compass
                    Compass.Default.Stop();
                    Compass.Default.ReadingChanged -= Compass_ReadingChanged;
                }
            }
        }
        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            // Update UI Label with compass state
            //CompassLabel.TextColor = Colors.Green;
            CurrentHeading = $"Compass: {e.Reading}";
        }
    }
}
