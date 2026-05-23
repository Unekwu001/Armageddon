using System.Text;
using System.Text.Json;
using Armageddon.Mobile.HelperServices;

namespace Armageddon.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    //private const string LoginUrl = "https://localhost:7061/api/v1/Auth/login";
    //private const string LoginUrl = "https://10.0.2.2:7061/api/v1/Auth/login";
    private const string BaseUrl = "https://10.0.2.2:7061";

    public LoginPage()
    {
        InitializeComponent();
    }

    public async Task LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlertAsync("Error", "Please enter email and password", "OK");
            return;
        }

        try
        {
            using var handler = new HttpClientHandler();

            // IMPORTANT: Bypass SSL validation for development (INSECURE — dev only)
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using var client = new HttpClient(handler);

            var payload = new
            {
                email = email.Trim(),
                password
            };

            var json = JsonSerializer.Serialize(payload);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v1/Auth/login");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode || apiResponse == null || !apiResponse.Success)
            {
                await DisplayAlertAsync("Login failed", apiResponse?.Message ?? responseJson, "OK");
                return;
            }

            string? token = apiResponse.Data;

            if (string.IsNullOrWhiteSpace(token))
            {
                await DisplayAlertAsync("Error", "No token returned from server", "OK");
                return;
            }

            await AuthNavigationService.SaveTokenAsync(token);
            await AuthNavigationService.NavigateByTokenAsync(token);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        await LoginAsync(EmailEntry.Text ?? "", PasswordEntry.Text ?? "");
    }

    private async void OnRegisterTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("register");
    }
}
