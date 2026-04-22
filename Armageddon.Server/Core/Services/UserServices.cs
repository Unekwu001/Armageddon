using Armageddon.Server.Core.Dtos;
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

namespace Armageddon.Server.Core.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterUserDto dto);
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



        public async Task<User> RegisterAsync(RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Password is required");

            if (await _userRepo.ExistsByEmailAsync(dto.Email))
                throw new ValidationException("Email already exists");

            if (await _userRepo.ExistsByUsernameAsync(dto.Username))
                throw new ValidationException("Username already exists");

            var passwordHash = HashPassword(dto.Password);

            var user = new User
            {
                UserName = dto.Username.Trim(),
                PasswordHash = passwordHash
            };

            var createdUser = await _userRepo.AddAsync(user);

            _logger.LogInformation("User registered: {Email}", createdUser.Email);

            return createdUser;
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
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            };
            return GenerateAccessToken(claims, _settings);
        }

    }
}
