using BLRB2B.Web.Models;

namespace BLRB2B.Web.Services;

public class ProductService
{
    private readonly HttpService _httpService;
    private const string BasePath = "api/products";

    public ProductService(HttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<PagedResult<ProductListModel>?> GetProductsAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
    {
        var searchQuery = string.IsNullOrEmpty(search) ? "" : $"&search={Uri.EscapeDataString(search)}";
        return await _httpService.GetAsync<PagedResult<ProductListModel>>($"{BasePath}?pageNumber={pageNumber}&pageSize={pageSize}{searchQuery}");
    }

    public async Task<ProductDetailModel?> GetProductAsync(int id)
    {
        return await _httpService.GetAsync<ProductDetailModel>($"{BasePath}/{id}");
    }

    public async Task<ProductDetailModel?> CreateProductAsync(CreateProductModel model)
    {
        return await _httpService.PostAsync<CreateProductModel, ProductDetailModel>(BasePath, model);
    }

    public async Task UpdateProductAsync(int id, UpdateProductModel model)
    {
        await _httpService.PutAsync($"{BasePath}/{id}", model);
    }

    public async Task DeleteProductAsync(int id)
    {
        await _httpService.DeleteAsync($"{BasePath}/{id}");
    }

    public async Task<List<ProductDropdownModel>?> GetProductDropdownAsync()
    {
        return await _httpService.GetAsync<List<ProductDropdownModel>>($"{BasePath}/dropdown");
    }
}

// Service-specific models for API requests
public class CreateProductModel
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }
}

public class UpdateProductModel
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
}
