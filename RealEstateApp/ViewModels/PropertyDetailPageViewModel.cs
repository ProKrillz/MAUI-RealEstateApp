using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(PropertyListItem), "MyPropertyListItem")]
public class PropertyDetailPageViewModel : BaseViewModel
{
    private readonly IPropertyService service;
    public PropertyDetailPageViewModel(IPropertyService service)
    {
        this.service = service;
        IsVisible = true;
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get { return _isVisible; }
        set { SetProperty(ref _isVisible, value); }
    }

    Property property;
    public Property Property { get => property; set { SetProperty(ref property, value); } }


    Agent agent;
    public Agent Agent { get => agent; set { SetProperty(ref agent, value); } }


    PropertyListItem propertyListItem;
    public PropertyListItem PropertyListItem
    {
        set
        {
            SetProperty(ref propertyListItem, value);
           
            Property = propertyListItem.Property;
            Agent = service.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);
        }
    }
    private CancellationTokenSource cts;

    private Command _startSpeck;
    public ICommand StartSpeck => _startSpeck ??= new Command(
        execute: async () => {
            IsVisible = false;
            cts = new CancellationTokenSource();
            await TextToSpeech.Default.SpeakAsync(Property.Description, cancelToken: cts.Token);
            IsVisible = true;
        });

    private Command _stopSpeck;
    public ICommand StopSpeck => _stopSpeck ??= new Command(
        execute: () => 
        {
            if (cts?.IsCancellationRequested ?? true)
                return;
            IsVisible = true;
            cts.Cancel();
        });

    private Command editPropertyCommand;
    public ICommand EditPropertyCommand => editPropertyCommand ??= new Command(async () =>
    {
        if (propertyListItem == null)
            return;

        await Shell.Current.GoToAsync(nameof(AddEditPropertyPage), true, new Dictionary<string, object>
        {
            {"MyProperty", Property }
        });
    });
 
}
