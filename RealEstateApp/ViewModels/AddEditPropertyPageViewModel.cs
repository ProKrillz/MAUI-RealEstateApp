using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(Property), "MyProperty")]
public class AddEditPropertyPageViewModel : BaseViewModel
{
    readonly IPropertyService service;

    public AddEditPropertyPageViewModel(IPropertyService service)
    {
        this.service = service;
        Agents = new ObservableCollection<Agent>(service.GetAgents());  
    }

    public string Mode { get; set; }

    #region PROPERTIES
    public ObservableCollection<Agent> Agents { get; }

    private Property _property;
    public Property Property
    {
        get => _property;
        set
        {
            SetProperty(ref _property, value);
            Title = Mode == "newproperty" ? "Add Property" : "Edit Property";

            if (_property.AgentId != null)
            {
                SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
            }
        }
    }

    private Agent _selectedAgent;
    public Agent SelectedAgent
    {
        get => _selectedAgent;
        set
        {
            if (Property != null)
            {
                _selectedAgent = value;
                Property.AgentId = _selectedAgent?.Id;
            }
        }
    }

    string statusMessage;
    public string StatusMessage
    {
        get { return statusMessage; }
        set { SetProperty(ref statusMessage, value); }
    }

    Color statusColor;
    public Color StatusColor
    {
        get { return statusColor; }
        set { SetProperty(ref statusColor, value); }
    }
    Color _statusBatteryColor;
    public Color StatusBatteryColor
    {
        get { return _statusBatteryColor; }
        set { SetProperty(ref _statusBatteryColor, value); }
    }
    private string _batteryStateLabel;
    public string BatteryStateLabel
    {
        get { return _batteryStateLabel; }
        set { SetProperty(ref _batteryStateLabel, value); }
    }

    private bool _flashlightSwitch = true;
    public bool FlashlightSwitch
    {
        get { return _flashlightSwitch; }
        set { SetProperty(ref _flashlightSwitch, value); }
    }
    #endregion

    #region Battery

    private bool _isBatteryWatched;

    private Command _watchBatteryCommand;
    public ICommand WatchBatteryCommand => _watchBatteryCommand ?? new Command(
        execute: () => 
        {
            if (!_isBatteryWatched)
            {
                Battery.Default.BatteryInfoChanged += Battery_BatteryInfoChanged;
            }
            else
            {
                Battery.Default.BatteryInfoChanged -= Battery_BatteryInfoChanged;
            }
            _isBatteryWatched = !_isBatteryWatched;
        });

    private void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
    {
        if (Battery.EnergySaverStatus == EnergySaverStatus.On)
        {
            StatusBatteryColor = Colors.Green;
            BatteryStateLabel = $"EnergySave is on";
            return;
        }
        if (e.State == BatteryState.Charging)
        {
            StatusBatteryColor = Colors.Yellow;
            BatteryStateLabel = $"Battery is {e.ChargeLevel * 100}% charged.";
            return;
        }
        if (e.ChargeLevel * 100 <= 20)
        {
            StatusBatteryColor = Colors.Red;
            BatteryStateLabel = $"Battery is under 20%: {e.ChargeLevel * 100}% charged.";
            return;
        }
        else
        {
            StatusBatteryColor = null;
            BatteryStateLabel = string.Empty;
        }
    }

    #endregion

    #region Flash

    private Command _flashlightSwitchCommand;
    public ICommand FlashlightSwitchCommand => _flashlightSwitchCommand ??= new Command(
        execute: async () => {
            try
            {
                if (FlashlightSwitch)
                {
                    await Flashlight.TurnOnAsync();
                    FlashlightSwitch = false;
                }
                else
                {
                    await Flashlight.TurnOffAsync();
                    FlashlightSwitch = true;
                }
            }
            catch (FeatureNotSupportedException ex)
            {
                // Handle not supported on device exception
            }
            catch (PermissionException ex)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to turn on/off flashlight
            }
        });

    #endregion

    private Command _checkInternet;
    public ICommand CheckInternet => _checkInternet ??= new Command(
        execute: async () => {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Error!", "No internet", "ok");
            }
        });

    #region Location


    private Command _getLocationFromAddressCommand;
    public ICommand GetLocationFromAddressCommand => _getLocationFromAddressCommand ??= new Command(
        execute: async () =>
        {
            if (!string.IsNullOrWhiteSpace(Property.Address))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Property.Address))
                    {
                        await Shell.Current.DisplayAlert("Error!", "Please enter an address", "ok");
                    }
                    else
                    {
                        Location location = (await Geocoding.GetLocationsAsync(Property.Address)).FirstOrDefault();
                        Property.Latitude = location.Latitude;
                        Property.Longitude = location.Longitude;
                        OnPropertyChanged(nameof(Property));
                    }

                }
                catch (Exception x)
                {
                    await Shell.Current.DisplayAlert("Error!", "An error wasn't handle properly", "ok");
                }
            }
        },
        canExecute: () => Connectivity.Current.NetworkAccess == NetworkAccess.Internet); 
        

    private Command _getAddressFromLocationCommand;
    public ICommand GetAddressFromLocationCommand => _getAddressFromLocationCommand ??= new Command(
        execute: async () =>
        {
            IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(Property.Latitude.Value, Property.Longitude.Value);
            Property.Address = placemarks.FirstOrDefault().ToString();
            OnPropertyChanged(nameof(Property));
        },
        canExecute: () => Property.Latitude.HasValue && Property.Longitude.HasValue);


    private Command _getCurrentLocation;
    public ICommand GetCurrentLocation => _getCurrentLocation ??= new Command(
        execute: async () => {
            try
            {
                Location location = await Geolocation.Default.GetLocationAsync();
                if (location is not null)
                {
                    Property.Latitude = location.Latitude;
                    Property.Longitude = location.Longitude;
                    OnPropertyChanged(nameof(Property));
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        },
        canExecute: () => Connectivity.Current.NetworkAccess == NetworkAccess.Internet);

    #endregion

    private Command savePropertyCommand;
    public ICommand SavePropertyCommand => savePropertyCommand ??= new Command(async () => await SaveProperty());
    private async Task SaveProperty()
    {
        if (IsValid() == false)
        {
            Vibration.Vibrate(5000);
            StatusMessage = "Please fill in all required fields";
            StatusColor = Colors.Red;
        }
        else
        {
            service.SaveProperty(Property);
            await Shell.Current.GoToAsync("///propertylist");
        }
    }

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Property.Address)
            || Property.Beds == null
            || Property.Price == null
            || Property.AgentId == null)
            return false;
        return true;
    }

    private Command cancelSaveCommand;
    public ICommand CancelSaveCommand => cancelSaveCommand ??= new Command(
        execute: async () => 
        {
            Vibration.Cancel();
            await Shell.Current.GoToAsync("..");
        });
    #region Compass

    private Command _goToCompassPageCommand;

    public ICommand GoToCompassPageCommand => _goToCompassPageCommand ??= new Command(
        execute: async () =>
        {
            if (string.IsNullOrWhiteSpace(Property.Aspect))
                return;

            await Shell.Current.GoToAsync(nameof(CompassPage), true, new Dictionary<string, object>
            {
                {"MyProperty", Property }
            });
        });

    #endregion

}
