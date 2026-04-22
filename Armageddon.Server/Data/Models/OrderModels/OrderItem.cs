using Armageddon.Server.Data.Models.AuditAndSoftDeleteModels;
using Armageddon.Server.Data.Models.ProductModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Armageddon.Server.Data.Models.OrderModels
{
    public class OrderItem : Base , ISoftDeletableEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // Product details at the time of purchase (snapshot)
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }

        public decimal UnitPrice { get; set; }        // Price at the time of order
        public int Quantity { get; set; }

        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
