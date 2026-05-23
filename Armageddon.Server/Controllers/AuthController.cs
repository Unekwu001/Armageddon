using Armageddon.Server.Common.Dtos;
using Armageddon.Server.Core.Services;
using Armageddon.Server.Data.Enums;
using Armageddon.Server.Data.Models.UserModels;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Armageddon.Server.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }



        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var response = await _userService.RegisterAsync(dto, dto.UserTypeEnum);
                return Ok(
                    ApiResponse<User>.Successful(
                    data: response,
                    message: "User created successfully",
                    metadata: new { CreatedAt = DateTime.UtcNow })
                    );

            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Error during registration");
                return BadRequest(ApiResponse.BadRequest("Bad Request", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to register user."));
            }
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _userService.LoginAsync(dto);

                return Ok(
                    ApiResponse<string>.Successful(
                    data: token,
                    message: "Login successful",
                    metadata: new { CreatedAt = DateTime.UtcNow })
                    );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ApiResponse.BadRequest("Bad Request", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to login user."));
            }
        }



        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest(ApiResponse.BadRequest("Invalid token"));

                var user = await _userService.GetByIdAsync(Guid.Parse(userIdClaim));

                if (user == null)
                    return BadRequest(ApiResponse.BadRequest("User not found"));

                return Ok(
                    ApiResponse<User>.Successful(
                    data: user,
                    message: "Current user retrieved successfully",
                    metadata: new { CreatedAt = DateTime.UtcNow })
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during fetching current user");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to fetch current user."));
            }
        }
    }
}
