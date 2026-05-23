using Armageddon.Server.Data.Models.AuditAndSoftDeleteModels;
using Armageddon.Server.Data.Models.PaymentModels;
using Armageddon.Server.Data.Models.UserModels;

namespace Armageddon.Server.Data.Models.OrderModels
{
    public class Order : Base, ISoftDeletableEntity
    {
        public Guid BuyerId { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentStatusId { get; set; }
        public int DeliveryStatusId { get; set; }
        public string? CryptoTransactionId { get; set; }
        public ICollection<OrderItem> Items { get; set; } = [];
        public DeliveryStatus DeliveryStatus { get; set; } = null!;
        public PaymentStatus PaymentStatus { get; set; } = null!;
        public User Buyer { get; set; } = null!;
    }
}
