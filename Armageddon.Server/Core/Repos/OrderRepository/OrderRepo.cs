using Armageddon.Server.Data;
using Armageddon.Server.Data.Db;
using Armageddon.Server.Data.Models.OrderModels;
using Microsoft.EntityFrameworkCore;

namespace Armageddon.Server.Core.Repos.OrderRepository
{
    public class OrderRepo : IOrderRepo
    {
        private readonly AppDbContext _context;

        public OrderRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.PaymentStatus)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.PaymentStatus)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetByBuyerAsync(Guid buyerId)
        {
            return await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.PaymentStatus)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetBySellerAsync(Guid sellerId)
        {
            return await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.PaymentStatus)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetBySellerAndDeliveryStatusAsync(Guid sellerId, int deliveryStatusId)
        {
            return await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.PaymentStatus)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o =>
                    o.DeliveryStatusId == deliveryStatusId &&
                    o.Items.Any(i => i.Product.SellerId == sellerId))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}