using Armageddon.Server.Common.Dtos;
using Armageddon.Server.Core.Repos.OrderRepository;
using Armageddon.Server.Core.Repos.ProductRepository;
using Armageddon.Server.Data.Enums;
using Armageddon.Server.Data.Models.OrderModels;
using System.ComponentModel.DataAnnotations;

namespace Armageddon.Server.Core.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetAllAsync();

        Task<OrderResponseDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<OrderResponseDto>> GetByBuyerAsync(Guid buyerId);

        Task<IEnumerable<SellerOrderResponseDto>> GetBySellerAsync(Guid sellerId);

        Task<IEnumerable<SellerOrderResponseDto>> GetBySellerAndDeliveryStatusAsync(Guid sellerId, int deliveryStatusId);

        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto, Guid buyerId);

        Task<bool> UpdateDeliveryStatusAsync(Guid orderId, int deliveryStatusId);

        Task<bool> DeleteAsync(Guid id);
    }

    public class OrderServices : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IProductRepo _productRepo;

        public OrderServices(IOrderRepo orderRepo, IProductRepo productRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            var orders = await _orderRepo.GetAllAsync();
            return orders.Select(MapToOrderResponseDto);
        }

        public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            return order == null ? null : MapToOrderResponseDto(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetByBuyerAsync(Guid buyerId)
        {
            var orders = await _orderRepo.GetByBuyerAsync(buyerId);
            return orders.Select(MapToOrderResponseDto);
        }

        public async Task<IEnumerable<SellerOrderResponseDto>> GetBySellerAsync(Guid sellerId)
        {
            var orders = await _orderRepo.GetBySellerAsync(sellerId);
            return orders.Select(order => MapToSellerOrderResponseDto(order, sellerId));
        }

        public async Task<IEnumerable<SellerOrderResponseDto>> GetBySellerAndDeliveryStatusAsync(Guid sellerId, int deliveryStatusId)
        {
            var orders = await _orderRepo.GetBySellerAndDeliveryStatusAsync(sellerId, deliveryStatusId);
            return orders.Select(order => MapToSellerOrderResponseDto(order, sellerId));
        }

        public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto, Guid buyerId)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new ValidationException("Order must contain at least one item.");

            var order = new Order
            {
                BuyerId = buyerId,
                PaymentStatusId = dto.PaymentStatusId,
                DeliveryStatusId = (int)DeliveryStatusEnum.NotStarted,
                CryptoTransactionId = dto.CryptoTransactionId,
                Items = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);

                if (product == null)
                    throw new ValidationException($"Product with ID '{item.ProductId}' was not found.");

                if (item.Quantity <= 0)
                    throw new ValidationException("Quantity must be greater than zero.");

                if (product.Stock < item.Quantity)
                    throw new ValidationException($"Insufficient stock for product '{product.Name}'.");

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    // SNAPSHOT
                    ProductName = product.Name,
                    ProductImageUrl = product.ImageUrl,

                    UnitPrice = product.PricePerGram,
                    Quantity = item.Quantity
                };

                totalAmount += orderItem.UnitPrice * orderItem.Quantity;

                product.Stock -= item.Quantity;

                order.Items.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            var createdOrder = await _orderRepo.GetByIdAsync(order.Id);

            return MapToOrderResponseDto(createdOrder!);
        }

        public async Task<bool> UpdateDeliveryStatusAsync(Guid orderId, int deliveryStatusId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order == null)
                return false;

            order.DeliveryStatusId = deliveryStatusId;

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
                return false;

            _orderRepo.Delete(order);
            await _orderRepo.SaveChangesAsync();

            return true;
        }

        private static OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                BuyerId = order.BuyerId,
                BuyerName = order.Buyer?.UserName ?? string.Empty,
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus?.Name ?? string.Empty,
                DeliveryStatus = order.DeliveryStatus?.Name ?? string.Empty,
                CryptoTransactionId = order.CryptoTransactionId,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    TotalPrice = i.UnitPrice * i.Quantity
                }).ToList()
            };
        }

        private static SellerOrderResponseDto MapToSellerOrderResponseDto(Order order, Guid sellerId)
        {
            var sellerItems = order.Items
                .Where(i => i.Product.SellerId == sellerId)
                .ToList();

            return new SellerOrderResponseDto
            {
                OrderId = order.Id,
                BuyerName = order.Buyer?.UserName ?? string.Empty,
                TotalAmount = sellerItems.Sum(i => i.UnitPrice * i.Quantity),
                PaymentStatus = order.PaymentStatus?.Name ?? string.Empty,
                DeliveryStatus = order.DeliveryStatus?.Name ?? string.Empty,
                CreatedAt = order.CreatedAt,
                Items = sellerItems.Select(i => new SellerOrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.UnitPrice * i.Quantity
                }).ToList()
            };
        }
    }
}