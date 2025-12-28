using Microsoft.AspNetCore.Mvc;
using BLRB2B.Application.Interfaces;
using BLRB2B.Application.Dto;

namespace BLRB2B.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var products = await _productService.GetAllProductsAsync();
        var pagedProducts = products.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return Ok(new { data = pagedProducts, total = products.Count(), pageNumber, pageSize });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID {id} not found" });
        }
        return Ok(product);
    }

    [HttpGet("sku/{sku}")]
    public async Task<IActionResult> GetBySku(string sku)
    {
        var product = await _productService.GetProductBySkuAsync(sku);
        if (product == null)
        {
            return NotFound(new { message = $"Product with SKU {sku} not found" });
        }
        return Ok(product);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var products = await _productService.GetActiveProductsAsync();
        return Ok(products);
    }

    [HttpGet("dropdown")]
    public async Task<IActionResult> GetDropdown()
    {
        var products = await _productService.GetActiveProductsAsync();
        var dropdownData = products.Select(p => new
        {
            id = p.Id,
            name = p.Name,
            sku = p.Sku,
            price = p.Price
        });
        return Ok(dropdownData);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if SKU is unique
        var isSkuUnique = await _productService.IsSkuUniqueAsync(dto.Sku);
        if (!isSkuUnique)
        {
            return BadRequest(new { message = "Product with this SKU already exists" });
        }

        try
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != dto.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        // Check if SKU is unique (excluding current product)
        var isSkuUnique = await _productService.IsSkuUniqueAsync(dto.Sku, dto.Id);
        if (!isSkuUnique)
        {
            return BadRequest(new { message = "Product with this SKU already exists" });
        }

        try
        {
            var product = await _productService.UpdateProductAsync(dto);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }
            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Product with ID {id} not found" });
        }
        return Ok(new { message = "Product deleted successfully" });
    }
}
