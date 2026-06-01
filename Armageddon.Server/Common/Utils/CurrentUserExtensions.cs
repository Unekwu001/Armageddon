using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Armageddon.Server.Common.Utils
{
    public static class CurrentUserExtensions
    {
        public static Guid GetCurrentUserId(this HubCallerContext context)
        {
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? context.User?.FindFirst("sub")?.Value;   // fallback for some tokens

            if (string.IsNullOrEmpty(userIdClaim))
                return Guid.Empty;

            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }
    }
}
