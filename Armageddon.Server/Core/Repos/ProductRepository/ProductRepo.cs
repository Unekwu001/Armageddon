
using Armageddon.Server.Data.Db;
using Armageddon.Server.Data.Enums;
using Armageddon.Server.Data.Models.ProductModels;
using Microsoft.EntityFrameworkCore;

namespace Armageddon.Server.Core.Repos.ProductRepository
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _context;

        public ProductRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductType>> GetAllTypesAsync()
        {
            return await _context.ProductTypes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Product?> GetByCodeAsync(string productCode)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.ProductCode == productCode);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            keyword = keyword?.Trim() ?? string.Empty;

            return await _context.Products
                .Where(x =>
                    x.ProductCode.Contains(keyword) ||
                    ((ProductTypeEnum)x.ProductTypeId)
                        .ToString()
                        .Contains(keyword))
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products
                .AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _context.Products
                .AnyAsync(x => x.ProductCode == productCode);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task AddRangeAsync(IEnumerable<Product> products)
        {
            await _context.Products.AddRangeAsync(products);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public void DeleteRange(IEnumerable<Product> products)
        {
            _context.Products.RemoveRange(products);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}