using Armageddon.Server.Data.Enums;

namespace Armageddon.Server.Common.Dtos
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public UserTypeEnum UserTypeEnum { get; set; } = default!; 
    }
}
