using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.AspNetCore.SignalR.Client;
using System.Timers;
using Armageddon.Mobile.Models;

namespace Armageddon.Mobile.Pages;

public partial class RequestSendingPage : ContentPage
{
    private HubConnection? _hubConnection;
    private System.Timers.Timer? _progressTimer;
    private Location? _currentLocation;



    public RequestSendingPage(string product = "", string quantity = "")
    {
        InitializeComponent();

        if (!string.IsNullOrEmpty(product) && !string.IsNullOrEmpty(quantity))
        {
            RequestSummaryLabel.Text = $"{quantity}g of {product}";
        }
        else
        {
            RequestSummaryLabel.Text = "Custom Request";
        }

        SetupMap();
        StartRequestCasting();
        ConnectToSignalRAsync();
    }




    private void SetupMap()
    {
#if ANDROID
        RequestMap.IsVisible = true;
        MapPlaceholder.IsVisible = false;
        InitializeMapAsync();
#else
        RequestMap.IsVisible = false;
        MapPlaceholder.IsVisible = true;
#endif
    }




    private async void InitializeMapAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            _currentLocation = await Geolocation.GetLocationAsync(request);

            if (_currentLocation != null)
            {
                RequestMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(_currentLocation.Latitude, _currentLocation.Longitude),
                    Distance.FromKilometers(3)));

                RequestMap.Pins.Add(new Pin
                {
                    Label = "You",
                    Address = "Current Location",
                    Type = PinType.Place,
                    Location = _currentLocation
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Location", "Could not get location.\nUsing default.", "OK");
        }
    }



    private async void ConnectToSignalRAsync()
    {
        try
        {
            // Reuse your existing service to get the token
            string? token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrEmpty(token))
            {
                await DisplayAlertAsync("Authentication Error", "Please login again.", "OK");
                return;
            }

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://10.0.2.2:7061/sellerHub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => handler;

                    // This is the key part - Pass the token automatically
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            // Receive nearby sellers
            _hubConnection.On<List<SellerLocationDto>>("ReceiveNearbySellers", sellers =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusLabel.Text = $"Found {sellers.Count} nearby sellers!";

                    var userPin = RequestMap.Pins.FirstOrDefault(p => p.Label == "You");
                    RequestMap.Pins.Clear();
                    if (userPin != null) RequestMap.Pins.Add(userPin);

                    foreach (var seller in sellers)
                    {
                        RequestMap.Pins.Add(new Pin
                        {
                            Label = seller.UserName,
                            Address = $"{seller.DistanceKm}km • {(seller.IsOnline ? "🟢 Online" : "⚪ Offline")}",
                            Type = PinType.Place,
                            Location = new Location(seller.Latitude, seller.Longitude)
                        });
                    }
                });
            });

            await _hubConnection.StartAsync();

            if (_currentLocation != null)
            {
                await _hubConnection.InvokeAsync("FindNearbySellers",
                    _currentLocation.Latitude,
                    _currentLocation.Longitude);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("SignalR Error", ex.Message, "OK");
        }
    }

    private void StartRequestCasting()
    {
        _progressTimer = new System.Timers.Timer(4000);
        _progressTimer.Elapsed += OnProgressTimerElapsed;
        _progressTimer.Start();
    }




    private void OnProgressTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusLabel.Text = "Expanding to more sellers...";
        });
    }




    private async void OnCancelClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Cancel Request",
            "Are you sure you want to cancel?", "Yes", "No");

        if (confirm)
        {
            _progressTimer?.Stop();
            if (_hubConnection != null)
                await _hubConnection.StopAsync();
            await Navigation.PopAsync();
        }
    }



    protected override async void OnDisappearing()
    {
        _progressTimer?.Stop();
        if (_hubConnection != null)
            await _hubConnection.StopAsync();
        base.OnDisappearing();
    }
}