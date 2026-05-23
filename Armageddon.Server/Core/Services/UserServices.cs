using Armageddon.Server.Common.Dtos;
using Armageddon.Server.Common.Utils;
using Armageddon.Server.Core.Repos.UserRepository;
using Armageddon.Server.Data.Enums;
using Armageddon.Server.Data.Models.UserModels;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Armageddon.Server.Core.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterUserDto dto, UserTypeEnum userTypeEnum);
        Task<string> LoginAsync(LoginDto dto);
        Task<User?> GetByIdAsync(Guid id);
    }
    public class UserServices : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly ILogger<UserServices> _logger;
        private readonly JwtSettings _settings;

        public UserServices(IUserRepo userRepo, ILogger<UserServices> logger, JwtSettings settings)
        {
            _userRepo = userRepo;
            _logger = logger;
            _settings = settings;
        }



        public async Task<User> RegisterAsync(RegisterUserDto dto, UserTypeEnum userTypeEnum)
        {
            try
            {
                ValidateInput(dto);

                var (email, username) = Normalize(dto);

                await EnsureNoDuplicatesAsync(email, username);

                var user = BuildUser(dto, email, username, userTypeEnum);

                var createdUser = await _userRepo.AddAsync(user);

                _logger.LogInformation("User registered: {Email}", createdUser.Email);

                return createdUser;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user");
                throw new ValidationException("An error occurred while processing your request");
            }
        }


        public async Task<string> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Invalid credentials");

            var passwordHash = HashPassword(dto.Password);

            var user = await _userRepo.ValidateUserCredentialsAsync(dto.Email, passwordHash);

            if (user == null)
                throw new ValidationException("Invalid email or password");

            var token = GenerateAccessToken(user);

            _logger.LogInformation("User logged in: {Email}", user.Email);

            return token;
        }




        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userRepo.GetByIdAsync(id);
        }




        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }




        private string GenerateAccessToken(List<Claim> claims, JwtSettings settings)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(settings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        private string GenerateAccessToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, ((UserTypeEnum)user.UserTypeId).ToString()),
                new Claim(ClaimTypes.Actor, user.UserCode ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            };
            return GenerateAccessToken(claims, _settings);
        }



        private void ValidateInput(RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Password is required");

            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ValidationException("Username is required");

            ValidateEmail(dto.Email);
            ValidateUsername(dto.Username);
            ValidatePassword(dto.Password);
        }







        private void ValidateEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!regex.IsMatch(email))
                throw new ValidationException("Invalid email format");
        }




        private void ValidateUsername(string username)
        {
            var regex = new Regex(@"^[A-Za-z0-9]{4,20}$");
            if (!regex.IsMatch(username))
                throw new ValidationException("Username must be 4-20 alphanumeric characters");
        }




        private void ValidatePassword(string password)
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
            if (!regex.IsMatch(password))
                throw new ValidationException("Password must be at least 8 characters and include uppercase, lowercase, number, and special character");
        }




        private (string Email, string Username) Normalize(RegisterUserDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var username = dto.Username.Trim().ToUpperInvariant();

            return (email, username);
        }




        private async Task EnsureNoDuplicatesAsync(string email, string username)
        {
            if (await _userRepo.ExistsByEmailAsync(email))
                throw new ValidationException("Email already exists");

            if (await _userRepo.ExistsByUsernameAsync(username))
                throw new ValidationException("Username already exists");
        }




        private User BuildUser(RegisterUserDto dto, string email, string username, UserTypeEnum userTypeEnum)
        {
            if (!Enum.IsDefined(typeof(UserTypeEnum), userTypeEnum) || (int)userTypeEnum == 0)
                throw new ValidationException("Invalid user type");
            return new User
            {
                UserName = username,
                Email = email,
                UserCode = Generator.GenerateUserCode().ToUpperInvariant(),
                UserTypeId = (int)userTypeEnum,
                PasswordHash = HashPassword(dto.Password)
            };
        }




    }
}
