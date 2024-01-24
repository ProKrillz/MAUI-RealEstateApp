using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class CompassPage : ContentPage
{
    private CompassViewModel vm;
    public CompassPage(CompassViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        this.vm = vm;

    }
    protected override void OnAppearing()
    {
        vm.ToggleCompass();
    }

}