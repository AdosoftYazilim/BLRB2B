using BLRB2B.Web.Models;

namespace BLRB2B.Web.Services;

public class OrderService
{
    private readonly HttpService _httpService;
    private const string BasePath = "api/orders";

    public OrderService(HttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<PagedResult<OrderListModel>?> GetOrdersAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null)
    {
        var searchQuery = string.IsNullOrEmpty(search) ? "" : $"&search={Uri.EscapeDataString(search)}";
        var statusQuery = string.IsNullOrEmpty(status) ? "" : $"&status={status}";
        return await _httpService.GetAsync<PagedResult<OrderListModel>>($"{BasePath}?pageNumber={pageNumber}&pageSize={pageSize}{searchQuery}{statusQuery}");
    }

    public async Task<OrderDetailModel?> GetOrderAsync(int id)
    {
        return await _httpService.GetAsync<OrderDetailModel>($"{BasePath}/{id}");
    }

    public async Task<OrderDetailModel?> CreateOrderAsync(CreateOrderModel model)
    {
        return await _httpService.PostAsync<CreateOrderModel, OrderDetailModel>(BasePath, model);
    }

    public async Task UpdateOrderStatusAsync(int id, UpdateOrderStatusModel model)
    {
        await _httpService.PutAsync($"{BasePath}/{id}/status", model);
    }

    public async Task DeleteOrderAsync(int id)
    {
        await _httpService.DeleteAsync($"{BasePath}/{id}");
    }

    public async Task<OrderSummaryModel?> GetOrderSummaryAsync()
    {
        return await _httpService.GetAsync<OrderSummaryModel>($"{BasePath}/summary");
    }
}

// Service-specific models for API requests
public class CreateOrderModel
{
    public int CustomerId { get; set; }
    public DateTime? RequiredDate { get; set; }
    public string? Notes { get; set; }
    public string? ShippingAddress { get; set; }
    public List<CreateOrderItemModel> Items { get; set; } = new();
}

public class CreateOrderItemModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}

public class UpdateOrderStatusModel
{
    public string Status { get; set; } = string.Empty;
}

public class OrderSummaryModel
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}
