using Armageddon.Server.Data.Models.AuditAndSoftDeleteModels;
using Armageddon.Server.Data.Models.ProductModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Armageddon.Server.Data.Models.OrderModels
{
    public class OrderItem : Base , ISoftDeletableEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        // SNAPSHOT
        public string ProductName { get; set; } = string.Empty;

        public string? ProductImageUrl { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
        public Product Product { get; set; } = null!;
        public Order Order { get; set; } = null!;
    }
}
