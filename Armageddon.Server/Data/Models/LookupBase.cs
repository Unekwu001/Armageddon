namespace Armageddon.Server.Data.Models
{
    public class LookupBase
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
