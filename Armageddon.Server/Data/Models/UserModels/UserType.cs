namespace Armageddon.Server.Data.Models.UserModels
{
    public class UserType : IHasIntegerId
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
    
}
