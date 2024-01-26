using System.Windows.Input;

namespace RealEstateApp.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
		private double _volumeValue;
		public double VolumeValue
		{
			get => _volumeValue; 
			set => SetProperty(ref _volumeValue, value); 
		}

        private Command _resetSettingsCommand;
        public ICommand ResetSettingsCommand => _resetSettingsCommand ??= new Command(
            execute: () =>
            {
                VolumeValue = 0.5;
            });
        public void SaveSettings()
		{
            Preferences.Default.Set("Volume", _volumeValue);
        }
		public void LoadSetting()
		{
			if (Preferences.Default.ContainsKey("Volume"))
			{
				VolumeValue = Preferences.Default.Get("Volume", -1.1);
            }
		}
    }
}
