using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Maui.Storage;

namespace Armageddon.Mobile.HelperServices;

public static class AuthNavigationService
{
    public static async Task SaveTokenAsync(string token)
    {
        await SecureStorage.SetAsync("auth_token", token);
    }

    public static string? GetUserTypeFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        return jwt.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.Role ||
            c.Type == "role" ||
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
    }

    public static async Task NavigateByTokenAsync(string token)
    {
        string? userType = GetUserTypeFromToken(token);

        if (string.Equals(userType, "Buyer", StringComparison.OrdinalIgnoreCase))
        {
            await Shell.Current.GoToAsync("buyer-home");
            return;
        }

        if (string.Equals(userType, "Seller", StringComparison.OrdinalIgnoreCase))
        {
            await Shell.Current.GoToAsync("seller-home");
            return;
        }

        await Shell.Current.DisplayAlertAsync("Error", $"Unknown account type: {userType}", "OK");
    }
}