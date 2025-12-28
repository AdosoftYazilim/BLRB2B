using BLRB2B.Domain.Entities;

namespace BLRB2B.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
}
