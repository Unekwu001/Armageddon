using Armageddon.Server.Common.Dtos;
using Armageddon.Server.Core.Repos.ProductRepository;
using Armageddon.Server.Data.Models.ProductModels;

namespace Armageddon.Server.Core.Services
{
    public interface IProductServices
    {
        Task<IEnumerable<string>> GetAllTypesAsync();
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();

        Task<ProductResponseDto?> GetByIdAsync(Guid id);

        Task<ProductResponseDto?> GetByCodeAsync(string productCode);

        Task<IEnumerable<ProductResponseDto>> SearchAsync(string keyword);

        Task<bool> ExistsAsync(Guid id);

        Task<ProductResponseDto> CreateAsync(CreateProductDto dto, Guid userId);

        Task<bool> UpdateAsync(Product product);

        Task<bool> DeleteAsync(Guid id);
    }

    public class ProductServices : IProductServices
    {
        private readonly IProductRepo _productRepo;

        public ProductServices(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }


        public async Task<IEnumerable<string>> GetAllTypesAsync()
        {
            var productTypes = await _productRepo.GetAllTypesAsync();

            return productTypes.Select(c=>c.Name);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var products = await _productRepo.GetAllAsync();

            return products.Select(MapToDto);
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductResponseDto?> GetByCodeAsync(string productCode)
        {
            var product = await _productRepo.GetByCodeAsync(productCode);

            return product == null ? null : MapToDto(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> SearchAsync(string keyword)
        {
            var products = await _productRepo.SearchAsync(keyword);

            return products.Select(MapToDto);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _productRepo.ExistsAsync(id);
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, Guid userId)
        {
            var product = new Product
            {
                Name = dto.Name,
                ProductCode = dto.ProductCode,
                ProductTypeId = dto.ProductTypeId,
                PricePerGram = dto.PricePerGram,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                SellerId = userId
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existing = await _productRepo.GetByIdAsync(product.Id);

            if (existing == null)
                return false;

            existing.ProductCode = product.ProductCode;
            existing.ProductTypeId = product.ProductTypeId;
            existing.PricePerGram = product.PricePerGram;

            _productRepo.Update(existing);
            await _productRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
                return false;

            _productRepo.Delete(product);
            await _productRepo.SaveChangesAsync();

            return true;
        }

        private static ProductResponseDto MapToDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                ProductTypeId = product.ProductTypeId,
                PricePerGram = product.PricePerGram
            };
        }
    }
}