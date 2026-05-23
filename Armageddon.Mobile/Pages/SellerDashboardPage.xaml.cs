using Armageddon.Mobile.Models;
using System.Collections.ObjectModel;

namespace Armageddon.Mobile.Pages
{
    public partial class SellerDashboardPage : ContentPage
    {
        public ObservableCollection<RequestItem> Requests { get; set; }

        public SellerDashboardPage()
        {
            InitializeComponent();

            Requests = new ObservableCollection<RequestItem>
        {
            new RequestItem { ItemName = "Rice", Quantity = "500g" },
            new RequestItem { ItemName = "Chicken", Quantity = "1kg" }
        };

            RequestsList.ItemsSource = Requests;
        }

        private async void OnAcceptClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button?.BindingContext as RequestItem;

            if (item != null)
            {
                await DisplayAlertAsync("Accepted", $"Accepted {item.ItemName}", "OK");
            }
        }
    }


}
