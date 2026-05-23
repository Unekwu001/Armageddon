using Armageddon.Mobile.HelperServices;
using System.Text;
using System.Text.Json;

namespace Armageddon.Mobile.Pages;

public partial class RegisterPage : ContentPage
{
    private const string RegisterUrl = "https://localhost:7061/api/v1/Auth/register";

    public RegisterPage()
    {
        InitializeComponent();
        UserTypePicker.SelectedIndex = 0;
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string username = UsernameEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";
        string userType = UserTypePicker.SelectedItem?.ToString() ?? "Buyer";

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlertAsync("Error", "Please fill all fields", "OK");
            return;
        }

        try
        {
            using var client = new HttpClient();

            var payload = new
            {
                email,
                username,
                password,
                userTypeEnum = userType
            };

            string json = JsonSerializer.Serialize(payload);

            using var request = new HttpRequestMessage(HttpMethod.Post, RegisterUrl);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                await DisplayAlertAsync("Registration failed", error, "OK");
                return;
            }

            await DisplayAlertAsync("Success", "Registration successful", "OK");
            var loginPage = new LoginPage();
            await loginPage.LoginAsync(email, password);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnLoginTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//login");
    }
}