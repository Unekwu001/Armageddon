using Armageddon.Server.Data.Models.OrderModels;

namespace Armageddon.Server.Core.Repos.OrderRepository
{
    public interface IOrderRepo
    {
        Task<IEnumerable<Order>> GetAllAsync();

        Task<Order?> GetByIdAsync(Guid id);

        Task<IEnumerable<Order>> GetByBuyerAsync(Guid buyerId);

        Task<IEnumerable<Order>> GetBySellerAsync(Guid sellerId);

        Task<IEnumerable<Order>> GetBySellerAndDeliveryStatusAsync(Guid sellerId, int deliveryStatusId);

        Task AddAsync(Order order);

        void Update(Order order);

        void Delete(Order order);

        Task<bool> ExistsAsync(Guid id);

        Task SaveChangesAsync();
    }
}