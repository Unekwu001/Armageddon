using Armageddon.Server.Common.Dtos;
using Armageddon.Server.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Armageddon.Server.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductServices productServices, ILogger<ProductController> logger)
        {
            _productServices = productServices;
            _logger = logger;
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetAllTypes()
        {
            try
            {
                var response = await _productServices.GetAllTypesAsync();

                return Ok(
                    ApiResponse<IEnumerable<string>>.Successful(
                        response,
                        "product types retrieved successfully",
                        metadata: new { CreatedAt = DateTime.UtcNow }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product types");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Failed to fetch product types."));
            }
        }


    }
}
