using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Armageddon.Mobile.Pages;

public partial class BuyerHomePage : ContentPage
{
    private readonly HttpClient _httpClient;

    public BuyerHomePage()
    {
        InitializeComponent();

        _httpClient = CreateConfiguredHttpClient();

        ProductTypePicker.SelectedIndexChanged += ProductTypePicker_SelectedIndexChanged;
        LoadProductTypesAsync();
    }


    private static HttpClient CreateConfiguredHttpClient()
    {
        var handler = new HttpClientHandler();

        // Only bypass SSL validation in Debug mode (Emulator / Development)
#if DEBUG
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://10.0.2.2:7061/")
        };

        return client;
    }
    private void ProductTypePicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // This forces the Picker to show the selected value properly on Android
        if (ProductTypePicker.SelectedIndex >= 0)
        {
            ProductTypePicker.Title = null;  
        }
    }
    private async void LoadProductTypesAsync()
    {
        try
        {
            // Retrieve token from SecureStorage
            string? token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                await DisplayAlertAsync("Authentication Error",
                    "Please login again. Token not found.", "OK");
                // Optional: Redirect to login
                // await Shell.Current.GoToAsync("login");
                return;
            }

            // Add Bearer Token to request
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<string>>>(
                "api/v1/Product/types");

            if (response?.Success == true && response.Data != null)
            {
                ProductTypePicker.ItemsSource = response.Data;

                ProductTypePicker.SelectedIndex = -1;
            }
            else
            {
                await DisplayAlertAsync("Error",
                    response?.Message ?? "Failed to load product types", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error",
                $"Could not load product types.\n\n{ex.Message}", "OK");
        }
    }
    private async void OnRequestClicked(object sender, EventArgs e)
    {
        if (ProductTypePicker.SelectedItem == null)
        {
            await DisplayAlertAsync("Error", "Please select a product type", "OK");
            return;
        }

        string selectedProduct = ProductTypePicker.SelectedItem.ToString() ?? "Null";
        string quantity = QuantityEntry.Text;

        try
        {
            await Shell.Current.GoToAsync("request-sending");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Navigation Error", ex.ToString(), "OK");
        }
    }

}
