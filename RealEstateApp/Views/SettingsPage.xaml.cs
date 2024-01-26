using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class SettingsPage : ContentPage
{
    SettingsPageViewModel _vm;
    public SettingsPage(SettingsPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
	}

    protected override void OnAppearing()
    {
        _vm.LoadSetting();
    }
    protected override void OnDisappearing()
    {
        _vm.SaveSettings();
    }
}