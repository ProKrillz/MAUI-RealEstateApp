using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class AddEditPropertyPage : ContentPage
{
	private readonly AddEditPropertyPageViewModel vm;
    public AddEditPropertyPage(AddEditPropertyPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this.vm = vm;
	}
    protected override void OnAppearing()
	{
		vm.CheckInternet.Execute(null);
		vm.WatchBatteryCommand.Execute(null);

    }
}