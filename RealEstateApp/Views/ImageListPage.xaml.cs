using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class ImageListPage : ContentPage
{
	ImageListPageModelView vm;
    public ImageListPage(ImageListPageModelView vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this.vm = vm;
	}
    protected override void OnAppearing()
    {
        vm.LoadImageCollection();
        vm.WatchAccelerometer();
    }
    protected override void OnDisappearing()
    {
        Accelerometer.Stop();
    }
}