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
        private string _rotationAngle;
        public string RotationAngle
        {
            get => _rotationAngle;
            set => SetProperty(ref _rotationAngle, value);
        }
        private string _currentAspect;
        public string CurrentAspect
        {
            get => _currentAspect;
            set => SetProperty(ref _currentAspect, value);
        }

        public void ToggleCompass()
        {
            if (Compass.Default.IsSupported)
            {
                if (!Compass.Default.IsMonitoring)
                {
                    Compass.Default.ReadingChanged += Compass_ReadingChanged;
                    Compass.Default.Start(SensorSpeed.UI);
                }
                else
                {
                    Compass.Default.Stop();
                    Compass.Default.ReadingChanged -= Compass_ReadingChanged;
                }
            }
        }
        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            RotationAngle = (-e.Reading.HeadingMagneticNorth).ToString();
            CurrentAspect =
            Property.Aspect = ConvertToText(e.Reading.HeadingMagneticNorth);
            CurrentHeading = $"Compass: {e.Reading.HeadingMagneticNorth}";
        }
        public static string ConvertToText(double heading)
        {
            // Ensure the heading is within the range [0, 360)
            heading = (heading % 360 + 360) % 360;

            if (heading >= 337.5 || heading < 22.5)
                return "North";
            else if (heading >= 22.5 && heading < 67.5)
                return "Northeast";
            else if (heading >= 67.5 && heading < 112.5)
                return "East";
            else if (heading >= 112.5 && heading < 157.5)
                return "Southeast";
            else if (heading >= 157.5 && heading < 202.5)
                return "South";
            else if (heading >= 202.5 && heading < 247.5)
                return "Southwest";
            else if (heading >= 247.5 && heading < 292.5)
                return "West";
            else if (heading >= 292.5 && heading < 337.5)
                return "Northwest";
            else
                return "Unknown"; // Handle any other cases
        }
    }
}
