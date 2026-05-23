using Armageddon.Mobile.Pages;

namespace Armageddon.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("buyer-home", typeof(BuyerHomePage));
        Routing.RegisterRoute("seller-home", typeof(SellerDashboardPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("welcome", typeof(WelcomePage));
        Routing.RegisterRoute("request-sending", typeof(RequestSendingPage));

    }
}
