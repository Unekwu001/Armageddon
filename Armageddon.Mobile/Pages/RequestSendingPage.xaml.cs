using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Armageddon.Mobile.Pages;

public partial class RequestSendingPage : ContentPage
{
    private System.Timers.Timer? _progressTimer;
    private Location? _currentLocation;

    public RequestSendingPage()
    {
        InitializeComponent();

#if ANDROID
        RequestMap.IsVisible = true;
        MapPlaceholder.IsVisible = false;
        InitializeMapAsync();
#else
        RequestMap.IsVisible = false;
        MapPlaceholder.IsVisible = true;
#endif
        StartRequestCasting();
    }

    private async void InitializeMapAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            _currentLocation = await Geolocation.GetLocationAsync(request);

            if (_currentLocation != null)
            {
                // Center map on user's location
                RequestMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(_currentLocation.Latitude, _currentLocation.Longitude),
                    Distance.FromKilometers(2))); // 2km initial radius

                // Add user pin
                var userPin = new Pin
                {
                    Label = "You",
                    Address = "Current Location",
                    Type = PinType.Place,
                    Location = new Location(_currentLocation.Latitude, _currentLocation.Longitude)
                };
                RequestMap.Pins.Add(userPin);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Location", "Unable to get current location.\nUsing default view.", ex.Message);
            // Fallback to a default location
            RequestMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Location(6.5244, 3.3792), Distance.FromKilometers(5)));
        }
    }

    private void StartRequestCasting()
    {
        _progressTimer = new System.Timers.Timer(4000);
        _progressTimer.Elapsed += OnProgressTimerElapsed;
        _progressTimer.AutoReset = false;
        _progressTimer.Start();
    }

    private void OnProgressTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusLabel.Text = "Expanding to more sellers...";
            Step1.TextColor = Colors.Gray;
            Step2.TextColor = Color.FromArgb("#00C853");
            Step2.Text = "● Expanding to all sellers";

            // Optional: Increase map radius
            if (_currentLocation != null)
            {
                RequestMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(_currentLocation.Latitude, _currentLocation.Longitude),
                    Distance.FromKilometers(8)));
            }
        });
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Cancel Request",
            "Are you sure you want to cancel?", "Yes", "No");

        if (confirm)
        {
            _progressTimer?.Stop();
            await Navigation.PopAsync();
        }
    }

    protected override void OnDisappearing()
    {
        _progressTimer?.Stop();
        base.OnDisappearing();
    }
}