namespace Armageddon.Mobile.Pages;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void OnBuyerTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//buyer-home");
    }

    private async void OnSellerTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//seller-home");
    }
}