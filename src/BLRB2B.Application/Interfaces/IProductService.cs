using BLRB2B.Application.Dto;

namespace BLRB2B.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto?> GetProductBySkuAsync(string sku);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
    Task<ProductDto> CreateProductAsync(ProductCreateDto dto);
    Task<ProductDto?> UpdateProductAsync(ProductUpdateDto dto);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);
}
