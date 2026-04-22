using Armageddon.Server.Data.Models.AuditAndSoftDeleteModels;
using Armageddon.Server.Data.Models.UserModels;

namespace Armageddon.Server.Data.Models.ProductModels
{
    public class Product : Base, ISoftDeletableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PricePerGram { get; set; } 
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public Guid SellerId { get; set; }
        public User Seller { get; set; } = null!;
    }
}
