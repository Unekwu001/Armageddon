namespace Armageddon.Server.Core.Dtos
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
