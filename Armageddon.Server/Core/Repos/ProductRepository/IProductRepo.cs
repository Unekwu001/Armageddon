using Armageddon.Server.Data.Models.ProductModels;

namespace Armageddon.Server.Core.Repos.ProductRepository
{
    public interface IProductRepo
    {
        Task<IEnumerable<ProductType>> GetAllTypesAsync();
        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(Guid id);

        Task<Product?> GetByCodeAsync(string productCode);

        Task<IEnumerable<Product>> SearchAsync(string keyword);

        Task<bool> ExistsAsync(Guid id);

        Task<bool> ExistsByCodeAsync(string productCode);

        Task AddAsync(Product product);

        Task AddRangeAsync(IEnumerable<Product> products);

        void Update(Product product);

        void Delete(Product product);

        void DeleteRange(IEnumerable<Product> products);

        Task<int> SaveChangesAsync();
    }
}