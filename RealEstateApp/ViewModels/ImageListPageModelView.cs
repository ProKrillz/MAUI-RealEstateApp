using System.Collections.ObjectModel;
using RealEstateApp.Models;

namespace RealEstateApp.ViewModels
{
    [QueryProperty(nameof(Property), "MyProperty")]

    public class ImageListPageModelView : BaseViewModel
    {
        public ObservableCollection<string> HouseImageCollection { get; set; } = new();

        private int _position;
        public int Position
        {
            get => _position; 
            set => SetProperty(ref _position, value); 
        }

        private Property _property;
        public Property Property 
        { 
            get => _property;  
            set => SetProperty(ref _property, value);  
        }
        public void LoadImageCollection()
        {
            foreach (string item in Property.ImageUrls)
                HouseImageCollection.Add(item);
        }
        public void WatchAccelerometer()
        {
            Accelerometer.Default.Start(SensorSpeed.UI);
            Accelerometer.Default.ShakeDetected += (sender, args) =>
            {
                if (Position == HouseImageCollection.Count - 1)
                {
                    Position = 0;
                }
                else
                {
                    Position++;
                }
            };
        }

    }
}
