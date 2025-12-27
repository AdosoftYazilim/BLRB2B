# BLRB2B - Kodlama Standartları ve Best Practices

## 📋 İçindekiler

1. [C# Coding Standards](#c-coding-standards)
2. [Blazor Guidelines](#blazor-guidelines)
3. [Entity Framework Guidelines](#entity-framework-guidelines)
4. [API Design Guidelines](#api-design-guidelines)
5. [Testing Guidelines](#testing-guidelines)
6. [Git Workflow](#git-workflow)
7. [Code Review Checklist](#code-review-checklist)

---

## 🔷 C# Coding Standards

### Naming Conventions

```csharp
// ✅ DO: PascalCase for classes, methods, properties
public class ProductService { }
public async Task<ProductDto> GetProductByIdAsync(int id) { }
public string ProductName { get; set; }

// ✅ DO: camelCase for local variables and parameters
var productName = "Test";
public void CalculateTax(decimal amount, decimal rate) { }

// ✅ DO: _camelCase for private fields
private readonly ILogger _logger;
private readonly IProductRepository _repository;

// ✅ DO: PascalCase with 'I' prefix for interfaces
public interface IProductService { }
public interface IRepository<T> where T : BaseEntity { }

// ✅ DO: PascalCase for constants
public const int MaxCartItems = 100;
public const string DefaultCurrency = "TRY";

// ❌ DON'T: Hungarian notation
int intCount = 0;  // Wrong
string strName = "";  // Wrong

// ❌ DON'T: Abbreviations
class ProdSvc { }  // Wrong - Use ProductService
```

### File Organization

```csharp
// File: Services/ProductService.cs

namespace BLRB2B.Application.Services;

// 1. Using statements (alphabetically, System.* first)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;
using Microsoft.Extensions.Logging;

// 2. Namespace (folder structure aligned)
namespace BLRB2B.Application.Services;

// 3. Class summary XML comment
/// <summary>
/// Service for managing products in the B2B system.
/// Handles product CRUD operations, pricing, and stock validation.
/// </summary>
public class ProductService : IProductService
{
    // 4. Fields (readonly first)
    private readonly ILogger<ProductService> _logger;
    private readonly IRepository<Product> _repository;
    private readonly IProductPriceService _priceService;

    // 5. Constructor
    public ProductService(
        ILogger<ProductService> logger,
        IRepository<Product> repository,
        IProductPriceService priceService)
    {
        _logger = logger;
        _repository = repository;
        _priceService = priceService;
    }

    // 6. Methods (public first, then private)
    // Methods grouped by functionality
}
```

### Method Design

```csharp
// ✅ DO: Use async/await properly
public async Task<ProductDto?> GetProductByIdAsync(int id)
{
    var product = await _repository.GetByIdAsync(id);
    return product == null ? null : MapToDto(product);
}

// ✅ DO: Use nullable return types appropriately
public async Task<ProductDto?> GetProductByIdAsync(int id) { }

// ✅ DO: Use specific exception types
if (product == null)
{
    throw new NotFoundException($"Product with ID {id} not found");
}

// ✅ DO: Validate inputs
public async Task CreateProductAsync(CreateProductDto dto)
{
    if (dto == null)
        throw new ArgumentNullException(nameof(dto));

    if (string.IsNullOrWhiteSpace(dto.ProductCode))
        throw new ValidationException("Product code is required");

    if (dto.ListPrice < 0)
        throw new ValidationException("Price cannot be negative");

    // ... rest of the logic
}

// ❌ DON'T: Catch-all exceptions
try
{
    // Some code
}
catch (Exception)  // Wrong - too broad
{
    // Swallow exception
}
```

### LINQ Guidelines

```csharp
// ✅ DO: Use LINQ for readable code
var activeProducts = _context.Products
    .Where(p => p.IsActive)
    .OrderBy(p => p.ProductName)
    .ToList();

// ✅ DO: Use Any() instead of Count() > 0 for existence check
if (products.Any())  // Good
if (products.Count() > 0)  // Bad - enumerates entire collection

// ✅ DO: Use FirstOrDefault() with null check
var product = products.FirstOrDefault(p => p.Id == id);
if (product != null) { }

// ❌ DON'T: Multiple enumerations
var products = GetProducts();
if (products.Any())  // First enumeration
{
    var first = products.First();  // Second enumeration
}

// ✅ DO: Cache enumeration
var products = GetProducts().ToList();  // Single enumeration
if (products.Any())
{
    var first = products.First();
}
```

### String Handling

```csharp
// ✅ DO: Use string interpolation
var message = $"Product {productName} created successfully";

// ✅ DO: Use StringBuilder for complex concatenation
var sb = new StringBuilder();
sb.AppendLine("Order Summary:");
sb.AppendLine($"Customer: {customerName}");
sb.AppendLine($"Total: {totalAmount}");

// ❌ DON'T: Use + for multiple concatenations
var message = "Order " + orderId + " created for " + customerName;

// ✅ DO: Use null-coalescing operator
var displayName = product.Name ?? "Unnamed";

// ✅ DO: Use null-conditional operator
var city = customer?.Address?.City;
var length = productName?.Length ?? 0;
```

### Async/Await Best Practices

```csharp
// ✅ DO: Use Async suffix
public async Task<Product> GetProductAsync(int id) { }

// ✅ DO: ConfigureAwait(false) in library code
public async Task<Product> GetProductAsync(int id)
{
    var product = await _repository
        .GetByIdAsync(id)
        .ConfigureAwait(false);
    return product;
}

// ✅ DO: Use CancellationToken
public async Task<List<Product>> GetProductsAsync(
    CancellationToken ct = default)
{
    return await _context.Products
        .ToListAsync(ct);
}

// ❌ DON'T: Async void (except event handlers)
public async void GetProduct()  // Wrong
{
}

// ✅ DO: Return Task
public async Task GetProduct()  // Correct
{
}
```

---

## 🎨 Blazor Guidelines

### Component Structure

```razor
@* ProductCard.razor *@

@* 1. Component directive *@
@implements IDisposable

@* 2. Inject dependencies *@
@inject IProductService ProductService
@inject NavigationManager Navigation

@* 3. Parameters *@
@code {
    [Parameter]
    public int ProductId { get; set; }

    [Parameter]
    public EventCallback<int> OnEdit { get; set; }
}

@* 4. Fields *@
@code {
    private ProductDto? _product;
    private bool _isLoading;
    private string? _errorMessage;
}

@* 5. Lifecycle methods *@
@code {
    protected override async Task OnInitializedAsync()
    {
        await LoadProductAsync();
    }
}

@* 6. Razor markup *@
<div class="product-card">
    @if (_isLoading)
    {
        <LoadingSpinner />
    }
    else if (_errorMessage != null)
    {
        <div class="alert alert-danger">@_errorMessage</div>
    }
    else if (_product != null)
    {
        <h3>@_product.ProductName</h3>
        <p>@_product.Description</p>
        <button @onclick="HandleEdit">Edit</button>
    }
</div>

@* 7. Methods *@
@code {
    private async Task LoadProductAsync()
    {
        _isLoading = true;
        try
        {
            _product = await ProductService.GetProductByIdAsync(ProductId);
        }
        catch (Exception ex)
        {
            _errorMessage = ex.Message;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task HandleEdit()
    {
        await OnEdit.InvokeAsync(ProductId);
    }

    public void Dispose()
    {
        // Cleanup
    }
}
```

### State Management

```razor
@* ✅ DO: Use cascading parameters for shared state *@

<CascadingValue Value="currentUser">
    <MainLayout />
</CascadingValue>

@* ✅ DO: Use State container for complex state *@
@inject ShoppingCartState ShoppingCart

<button @onclick="AddToCart">Add to Cart (@ShoppingCart.ItemCount)</button>

@code {
    private async Task AddToCart()
    {
        await ShoppingCart.AddItemAsync(ProductId);
        StateHasChanged();  // Notify component to re-render
    }
}
```

### Performance Best Practices

```razor
@* ✅ DO: Override ShouldRender for optimization *@

@code {
    protected override bool ShouldRender()
    {
        // Only re-render if data changed
        return _hasChanges;
    }
}

@* ✅ DO: Use @key for list rendering *@

@foreach (var product in products)
{
    <ProductCard @key="product.ProductId" Product="@product" />
}

@* ✅ DO: Use virtualization for large lists *@

<Virtualize Items="@products" Context="product">
    <ProductCard Product="@product" />
</Virtualize>
```

---

## 🗄️ Entity Framework Guidelines

### DbContext Usage

```csharp
// ✅ DO: Use dependency injection
public class ProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ DO: Use context per request (scoped lifetime)
    public async Task<Product> GetProductAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
```

### Query Optimization

```csharp
// ✅ DO: Use AsNoTracking for read-only queries
var products = await _context.Products
    .AsNoTracking()
    .Where(p => p.IsActive)
    .ToListAsync();

// ✅ DO: Use Include for related data
var orders = await _context.Orders
    .Include(o => o.Customer)
    .Include(o => o.Items)
        .ThenInclude(i => i.Product)
    .ToListAsync();

// ✅ DO: Use SplitQuery for complex includes (EF Core 5+)
var orders = await _context.Orders
    .Include(o => o.Customer)
    .Include(o => o.Items)
        .ThenInclude(i => i.Product)
    .AsSplitQuery()
    .ToListAsync();

// ❌ DON'T: N+1 query problem
foreach (var order in orders)
{
    var customer = await _context.Customers  // Wrong - N queries
        .FirstOrDefaultAsync(c => c.Id == order.CustomerId);
}

// ✅ DO: Eager loading instead
var orders = await _context.Orders
    .Include(o => o.Customer)
    .ToListAsync();
```

### Transaction Management

```csharp
// ✅ DO: Use explicit transactions for complex operations
public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // 1. Create order
        var order = new Order { /* ... */ };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // 2. Add items
        foreach (var itemDto in dto.Items)
        {
            var item = new OrderItem { /* ... */ };
            _context.OrderItems.Add(item);
        }
        await _context.SaveChangesAsync();

        // 3. Update stock
        foreach (var itemDto in dto.Items)
        {
            var stock = await _context.Stock
                .FirstOrDefaultAsync(s =>
                    s.ProductId == itemDto.ProductId);
            if (stock != null)
            {
                stock.Quantity -= itemDto.Quantity;
            }
        }
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
        return order;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### Migration Best Practices

```bash
# ✅ DO: Create descriptive migration names
dotnet ef migrations add AddProductImagesTable

# ✅ DO: Review generated migration code
# Before applying, check the Up() and Down() methods

# ✅ DO: Test migrations in development
dotnet ef database update

# ✅ DO: Generate SQL script for production
dotnet ef migrations script --output migration.sql

# ❌ DON'T: Modify existing migrations
# If you need to change a migration, create a new one instead
```

---

## 🌐 API Design Guidelines

### Response Format

```csharp
// ✅ DO: Use consistent response wrapper
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
}

public class ApiError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}

// ✅ DO: Return appropriate status codes
[HttpGet("{id}")]
public async Task<ActionResult<ProductDto>> GetProduct(int id)
{
    var product = await _service.GetProductByIdAsync(id);

    if (product == null)
        return NotFound(new ApiResponse<object>
        {
            Success = false,
            Error = new ApiError
            {
                Code = "PRODUCT_NOT_FOUND",
                Message = $"Product with ID {id} not found"
            }
        });

    return Ok(new ApiResponse<ProductDto>
    {
        Success = true,
        Data = product
    });
}
```

### Validation

```csharp
// ✅ DO: Use FluentValidation
public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.ProductCode)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Z0-9-]+$")
            .WithMessage("Product code must be alphanumeric");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ListPrice)
            .GreaterThan(0);

        RuleFor(x => x.MinOrderQuantity)
            .GreaterThan(0);
    }
}

// Register in Program.cs
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

// Use in controller
[HttpPost]
public async Task<ActionResult> CreateProduct(CreateProductDto dto)
{
    var validationResult = await _validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        return BadRequest(new ApiResponse<object>
        {
            Success = false,
            Error = new ApiError
            {
                Code = "VALIDATION_ERROR",
                Message = "Validation failed",
                Details = string.Join(", ",
                    validationResult.Errors.Select(e => e.ErrorMessage))
            }
        });
    }

    // ... create product
}
```

---

## 🧪 Testing Guidelines

### Unit Tests

```csharp
// ✅ DO: Follow AAA pattern (Arrange, Act, Assert)
public class ProductServiceTests
{
    [Fact]
    public async Task GetProductByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var productId = 1;
        var mockRepo = new Mock<IRepository<Product>>();
        var product = new Product { ProductId = productId, ProductName = "Test" };
        mockRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var service = new ProductService(mockRepo.Object, Mock.Of<ILogger>());

        // Act
        var result = await service.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal("Test", result.ProductName);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductNotFound_ReturnsNull()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(mockRepo.Object, Mock.Of<ILogger>());

        // Act
        var result = await service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}
```

### Integration Tests

```csharp
// ✅ DO: Test with real database (use test database)
public class OrderIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public OrderIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 2 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content
            .ReadFromJsonAsync<ApiResponse<OrderDto>>();
        Assert.True(result!.Success);
    }
}
```

---

## 🔄 Git Workflow

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

### Examples

```
feat(product): add product image upload

- Implement image upload endpoint
- Add image validation
- Update database schema

Closes #123

```

```
fix(order): resolve stock reservation issue

Stock was not being reserved when order was created.
Now correctly reserves stock on order creation.

Fixes #156
```

### Branch Naming

```
feature/ISSUE-ID-short-description
bugfix/ISSUE-ID-short-description
hotfix/ISSUE-ID-short-description

Examples:
feature/45-product-catalog
bugfix/78-stock-reservation
hotfix/101-payment-failure
```

---

## ✅ Code Review Checklist

### Functionality
- [ ] Does the code implement the requirements?
- [ ] Are edge cases handled?
- [ ] Is error handling appropriate?
- [ ] Are there any potential null reference exceptions?

### Performance
- [ ] Are there any N+1 query problems?
- [ ] Is caching used where appropriate?
- [ ] Are async/await used correctly?
- [ ] Are there any memory leaks?

### Security
- [ ] Is user input validated?
- [ ] Are sensitive data logged?
- [ ] Are SQL injection vulnerabilities avoided?
- [ ] Is authorization checked?

### Code Quality
- [ ] Is the code readable and well-documented?
- [ ] Are naming conventions followed?
- [ ] Is the code DRY (Don't Repeat Yourself)?
- [ ] Are magic numbers/strings replaced with constants?

### Testing
- [ ] Are unit tests included?
- [ ] Do tests cover happy path and error cases?
- [ ] Are tests maintainable?

### Documentation
- [ ] Is XML documentation added for public APIs?
- [ ] Are complex algorithms explained?
- [ ] Are TODO/FIXME comments addressed or documented?

---

## 📦 Useful Snippets

### Logger Template

```csharp
// Information
_logger.LogInformation("Product {ProductId} created by user {UserId}",
    product.Id, userId);

// Warning
_logger.LogWarning("Low stock detected for product {ProductId}. Quantity: {Quantity}",
    productId, quantity);

// Error
_logger.LogError(ex, "Failed to process payment for order {OrderId}",
    orderId);
```

### Validator Template

```csharp
public class CreateEntityValidator : AbstractValidator<CreateEntityDto>
{
    public CreateEntityValidator()
    {
        RuleFor(x => x.Property)
            .NotEmpty()
            .WithMessage("Property is required")
            .MaximumLength(100)
            .WithMessage("Property max length is 100");

        RuleFor(x => x.NumericProperty)
            .GreaterThan(0)
            .WithMessage("Must be greater than 0");
    }
}
```

### Repository Pattern Template

```csharp
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
```
