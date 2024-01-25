﻿using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Text.Json;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

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
    private Command _goToImageListPageCommand;
    public ICommand GoToImageListPageCommand => _goToImageListPageCommand ??= new Command(
        execute: async () =>
        {
            await Shell.Current.GoToAsync(nameof(ImageListPage), true, new Dictionary<string, object>
            {
                {"MyProperty", Property }
            });
        });

    private Command _phoneCallMsgCommand;
    public ICommand PhoneCallMsgCommand => _phoneCallMsgCommand ??= new Command(
        execute: async () =>
        {
            string action = await Shell.Current.DisplayActionSheet($"{Property.Vendor.Phone}", "Cancel", null, "Call", "SMS");
            switch (action)
            {
                case "Call":
                    if (PhoneDialer.Default.IsSupported)
                        PhoneDialer.Default.Open($"{Property.Vendor.Phone}");
                    break;
                case "SMS":
                    if (Sms.Default.IsComposeSupported)
                    {
                        string[] recipients = new[] { $"{Property.Vendor.Phone}" };
                        string text = $"Hej {Property.Vendor.FullName}, angående {Property.Address}. Tag hensyn til manglende feature.";

                        var message = new SmsMessage(text, recipients);

                        await Sms.Default.ComposeAsync(message);
                    }
                    break;
                default:
                    break;
            }

        });
    private Command _mailCommand;

    public ICommand MailCommand => _mailCommand ??= new Command(
        execute: async () =>
        {
            if (Email.Default.IsComposeSupported)
            {

                string subject = $"Hello {Property.Vendor.FullName}";
                string body = "It was great to see you last weekend.";
                string[] recipients = new[] { $"{Property.Vendor.Email}" };

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string>(recipients)
                };
                var folder = FileSystem.AppDataDirectory;
                var attachmentFilePath = Path.Combine(folder, "property.txt");
                File.WriteAllText(attachmentFilePath, $"{Property.Address}");

                message.Attachments.Add(new EmailAttachment(attachmentFilePath));

                await Email.Default.ComposeAsync(message);
            }
        });
    private Command _openMapsCommand;
    public ICommand OpenMapsCommand => _openMapsCommand ??= new Command(
        execute: async () =>
        {
            var location = new Location(Property.Latitude.Value, Property.Longitude.Value);
            var options = new MapLaunchOptions { Name = Property.Address };

            try
            {
                await Map.Default.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                // No map application available to open
            }
        });
    private Command _openMapsNavigationCommand;
    public ICommand OpenMapsNavigationCommand => _openMapsNavigationCommand ??= new Command(
        execute: async () =>
        {
            var location = new Location(Property.Latitude.Value, Property.Longitude.Value);
            var options = new MapLaunchOptions
            {
                Name =  Property.Address,
                NavigationMode = NavigationMode.Transit
            };

            try
            {
                await Map.Default.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                // No map application available to open
            }
        });
    private Command _openBrowserCommand;
    public ICommand OpenBrowserCommand => _openBrowserCommand ??= new Command(
        execute: async () =>
        {
            try
            {
                Uri uri = new Uri(Property.NeighbourhoodUrl);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // An unexpected error occurred. No browser may be installed on the device.
            }
        });
    private Command _openFileContactCommand;
    public ICommand OpenFileContactCommand => _openFileContactCommand ??= new Command(
        execute: async () =>
            await Launcher.Default.OpenAsync(new OpenFileRequest() 
                { 
                    File = new ReadOnlyFile(Path.Combine(FileSystem.AppDataDirectory, Property.ContractFilePath)) 
                }));

    private Command _shareTextAndLinkCommand;
    public ICommand ShareTextAndLinkCommand => _shareTextAndLinkCommand ??= new Command(
        execute: async () =>
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = "Share Property",
                Text = $"{Property.Address}, Price: {Property.Price}, beds: {Property.Beds}",
                Uri = Property.NeighbourhoodUrl,
                Subject = "A property you may be interested in"
            });
        });
    private Command _shareFileCommand;
    public ICommand ShareFileCommand => _shareFileCommand ??= new Command(
        execute: async () =>
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Share text file",
                File = new ShareFile(Path.Combine(FileSystem.AppDataDirectory, Property.ContractFilePath))
            });
        });
    private Command _shareJsonCommand;
    public ICommand ShareJsonCommand => _shareJsonCommand ??= new Command(
        execute: async () =>
        {
            //Virker ikk
            string jsonString = JsonSerializer.Serialize(Property);
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = "sut",
                Text = $"hey",
                Uri = "www.google.com",
                Subject = jsonString
            });
        }
        );
}
