using AutoMapper;
using BLRB2B.Application.Dto;
using BLRB2B.Application.Interfaces;
using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;

namespace BLRB2B.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> GetProductBySkuAsync(string sku)
    {
        var product = await _productRepository.GetBySkuAsync(sku);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateDto dto)
    {
        // Check if SKU is unique
        if (!await _productRepository.IsSkuUniqueAsync(dto.Sku))
        {
            throw new InvalidOperationException($"Product with SKU '{dto.Sku}' already exists.");
        }

        var product = _mapper.Map<Product>(dto);
        var createdProduct = await _productRepository.AddAsync(product);
        return _mapper.Map<ProductDto>(createdProduct);
    }

    public async Task<ProductDto?> UpdateProductAsync(ProductUpdateDto dto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(dto.Id);
        if (existingProduct == null)
        {
            return null;
        }

        // Check if SKU is unique
        if (!await _productRepository.IsSkuUniqueAsync(dto.Sku, dto.Id))
        {
            throw new InvalidOperationException($"Product with SKU '{dto.Sku}' already exists.");
        }

        _mapper.Map(dto, existingProduct);
        existingProduct.UpdatedAt = DateTime.UtcNow;
        await _productRepository.UpdateAsync(existingProduct);
        return _mapper.Map<ProductDto>(existingProduct);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product);
        return true;
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
    {
        return await _productRepository.IsSkuUniqueAsync(sku, excludeId);
    }
}
