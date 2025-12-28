using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;
using BLRB2B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BLRB2B.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Sku == sku);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
    {
        return excludeId == null
            ? !await _dbSet.AnyAsync(p => p.Sku == sku)
            : !await _dbSet.AnyAsync(p => p.Sku == sku && p.Id != excludeId.Value);
    }
}
