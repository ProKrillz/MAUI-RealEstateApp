using Microsoft.Maui.Devices.Sensors;
using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;
public class PropertyListPageViewModel : BaseViewModel
{
    public ObservableCollection<PropertyListItem> PropertiesCollection { get; } = new();

    private readonly IPropertyService service;

    public PropertyListPageViewModel(IPropertyService service)
    {
        Title = "Property List";
        this.service = service;
    }

    bool isRefreshing;
    public bool IsRefreshing
    {
        get => isRefreshing;
        set => SetProperty(ref isRefreshing, value);
    }

    private Command _sortCommand;
    public ICommand SortCommand => _sortCommand ??= new Command(
        execute: () =>
        {
            List<PropertyListItem> list = PropertiesCollection.ToList();
            PropertiesCollection.Clear();
            foreach (var item in list.OrderBy(x => x.Distance))
                PropertiesCollection.Add(item);
        },
        canExecute: () => Geolocation.GetLastKnownLocationAsync() != null
        );

    private Command getPropertiesCommand;
    public ICommand GetPropertiesCommand => getPropertiesCommand ??= new Command(async () => await GetPropertiesAsync());

    async Task GetPropertiesAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            List<Property> properties = service.GetProperties();

            if (PropertiesCollection.Count != 0)
                PropertiesCollection.Clear();

            Location location = await Geolocation.GetLocationAsync();
            foreach (Property property in properties)
            {
                PropertiesCollection.Add(new PropertyListItem(property, Location.CalculateDistance(property.Latitude.Value, property.Longitude.Value, location, DistanceUnits.Kilometers)));
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    private Command _goToDetailsCommand;
    public ICommand GoToDetailsCommand => _goToDetailsCommand ??= new Command<PropertyListItem>(async (propertyListItem) =>
    {
        if (propertyListItem == null)
            return;

        await Shell.Current.GoToAsync(nameof(PropertyDetailPage), true, new Dictionary<string, object>
        {
            {"MyPropertyListItem", propertyListItem }
        });
    });

    private Command goToAddPropertyCommand;
    public ICommand GoToAddPropertyCommand => goToAddPropertyCommand ??= new Command(async () => await GotoAddProperty());
    async Task GotoAddProperty()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}?mode=newproperty", true, new Dictionary<string, object>
        {
            {"MyProperty", new Property() }
        });
    }
}
