using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Armageddon.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        protected Guid CurrentUserId => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
          ?? throw new UnauthorizedAccessException("User ID not found in token"));
        protected string CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value
          ?? throw new UnauthorizedAccessException("Email not found in token");
        protected string CurrentUserPhone => User.FindFirst(ClaimTypes.MobilePhone)?.Value
          ?? throw new UnauthorizedAccessException("Phone number not found in token");
        protected string CurrentUserRole => User.FindFirst(ClaimTypes.Role)?.Value
          ?? throw new UnauthorizedAccessException("User Role not found in token");
        protected string CurrentUserCode => User.FindFirst(ClaimTypes.Actor)?.Value
                    ?? throw new UnauthorizedAccessException("User Code not found in token");
        protected string CurrentUserName => User.FindFirst(ClaimTypes.Name)?.Value
                    ?? throw new UnauthorizedAccessException("User Name not found in token");

    }
}
