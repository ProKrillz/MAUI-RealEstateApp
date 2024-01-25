using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class HeightCalculatorPage : ContentPage
{
	HeightCalculatorPageViewModel vm;
    public HeightCalculatorPage(HeightCalculatorPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this.vm = vm;
	}
    protected override void OnAppearing()
    {
        vm.ToggleBarometer();	

    }
}