using Armageddon.Server.Data.Models.AuditAndSoftDeleteModels;

namespace Armageddon.Server.Data.Models.UserModels
{
    public class User : Base,ISoftDeletableEntity
    {
        public required string UserCode { get; set; }
        public required string PasswordHash { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WalletAddress { get; set; }
        public int UserTypeId { get; set; }
        public virtual UserType UserType { get; set; } = null!;


    }
}
