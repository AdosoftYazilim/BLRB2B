namespace BLRB2B.Web.Models;

// Dashboard Models
public class DashboardSummaryModel
{
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrders { get; set; }
    public int LowStockProducts { get; set; }
}

public class RecentOrderModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
}

public class TopCustomerModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal TotalPurchased { get; set; }
    public int OrderCount { get; set; }
}

public class LowStockProductModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }
}

public class SalesChartModel
{
    public List<ChartDataPoint> MonthlyData { get; set; } = new();
    public List<ChartDataPoint> WeeklyData { get; set; } = new();
}

public class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

// View Models for Pages
public class BaseViewModel
{
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}

public class DashboardViewModel : BaseViewModel
{
    public DashboardSummaryModel Summary { get; set; } = new();
    public List<RecentOrderModel> RecentOrders { get; set; } = new();
    public List<TopCustomerModel> TopCustomers { get; set; } = new();
    public List<LowStockProductModel> LowStockProducts { get; set; } = new();
}

public class ProductListViewModel : BaseViewModel
{
    public PagedResult<ProductListModel> Products { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ProductViewModel : BaseViewModel
{
    public ProductDetailModel? Product { get; set; }
    public List<CategoryModel> Categories { get; set; } = new();
}

public class CustomerListViewModel : BaseViewModel
{
    public PagedResult<CustomerListModel> Customers { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CustomerViewModel : BaseViewModel
{
    public CustomerDetailModel? Customer { get; set; }
}

public class OrderListViewModel : BaseViewModel
{
    public PagedResult<OrderListModel> Orders { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public string? StatusFilter { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class OrderViewModel : BaseViewModel
{
    public OrderDetailModel? Order { get; set; }
    public List<CustomerDropdownModel> Customers { get; set; } = new();
    public List<ProductDropdownModel> Products { get; set; } = new();
}

// Support Models
public class CategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class SelectListItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Disabled { get; set; }
}

// Paged Result
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

// Product Models
public class ProductListModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ProductDetailModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class ProductDropdownModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public decimal Price { get; set; }
}

// Customer Models
public class CustomerListModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CustomerGroupName { get; set; }
    public bool IsActive { get; set; }
    public string? ContactPerson { get; set; }
    public string? City { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CustomerDetailModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? CustomerGroupId { get; set; }
    public string? CustomerGroupName { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class CustomerDropdownModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? City { get; set; }
}

// Order Models
public class OrderListModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

public class OrderDetailModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ShippingAddress { get; set; }
    public List<OrderItemDetailModel> Items { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class OrderItemDetailModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalPrice { get; set; }
}

// Order Summary Model
public class OrderSummaryModel
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}
