using Armageddon.Server.Common.Dtos;
using Armageddon.Server.Core.Services;
using Armageddon.Server.Data.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Armageddon.Server.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")] 
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }
        [HttpGet]
        [Authorize(Roles = nameof(UserTypeEnum.Admin) + "," + nameof(UserTypeEnum.SuperAdmin))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _orderService.GetAllAsync();

                return Ok(
                    ApiResponse<IEnumerable<OrderResponseDto>>.Successful(
                        response,
                        "Orders retrieved successfully",
                        metadata: new { CreatedAt = DateTime.UtcNow }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to fetch orders."));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var response = await _orderService.GetByIdAsync(id);

                if (response == null)
                    return NotFound(ApiResponse.NotFound("Order not found"));

                return Ok(
                    ApiResponse<OrderResponseDto>.Successful(
                        response,
                        "Order retrieved successfully",
                        metadata: new { CreatedAt = DateTime.UtcNow }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to fetch order."));
            }
        }

        [HttpGet("buyer")]
        [Authorize(Roles = nameof(UserTypeEnum.Buyer))]
        public async Task<IActionResult> GetBuyerOrders()
        {
            try
            {
                var response = await _orderService.GetByBuyerAsync(CurrentUserId);

                return Ok(
                    ApiResponse<IEnumerable<OrderResponseDto>>.Successful(
                        response,
                        "Buyer orders retrieved successfully",
                        metadata: new { CreatedAt = DateTime.UtcNow }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching buyer orders");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to fetch buyer orders."));
            }
        }

        [HttpGet("seller")]
        [Authorize(Roles = nameof(UserTypeEnum.Seller))]
        public async Task<IActionResult> GetSellerOrders()
        {
            try
            {
                var response = await _orderService.GetBySellerAsync(CurrentUserId);

                return Ok(
                    ApiResponse<IEnumerable<SellerOrderResponseDto>>.Successful(
                        response,
                        "Seller orders retrieved successfully",
                        metadata: new { CreatedAt = DateTime.UtcNow }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching seller orders");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to fetch seller orders."));
            }
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserTypeEnum.Buyer))]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            try
            {
                var response = await _orderService.CreateAsync(dto, CurrentUserId);

                return Ok(
                    ApiResponse<OrderResponseDto>.Successful(
                        response,
                        "Order created successfully",
                        metadata: new { CreatedAt = DateTime.UtcNow }));
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation error while creating order");

                return BadRequest(
                    ApiResponse.ValidationError(
                        errors: [ex.Message]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to create order."));
            }
        }

        [HttpPut("{id:guid}/delivery-status/{deliveryStatusId:int}")]
        [Authorize(Roles = nameof(UserTypeEnum.Seller) + "," + nameof(UserTypeEnum.Admin))]
        public async Task<IActionResult> UpdateDeliveryStatus(
            Guid id,
            int deliveryStatusId)
        {
            try
            {
                var updated = await _orderService
                    .UpdateDeliveryStatusAsync(id, deliveryStatusId);

                if (!updated)
                    return NotFound(ApiResponse.NotFound("Order not found"));

                return Ok(
                    ApiResponse.Successful(
                        data: null,
                        message: "Delivery status updated successfully",
                        metadata: new { CreatedAt = DateTime.UtcNow }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delivery status");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to update delivery status."));
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = nameof(UserTypeEnum.Admin) + "," + nameof(UserTypeEnum.SuperAdmin))]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _orderService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(ApiResponse.NotFound("Order not found"));

                return Ok(
                    ApiResponse.Successful(
                        data: null,
                        message: "Order deleted successfully",
                        metadata: new { CreatedAt = DateTime.UtcNow }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to delete order."));
            }
        }
    }
}