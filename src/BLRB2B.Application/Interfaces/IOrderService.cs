using BLRB2B.Application.Dto;

namespace BLRB2B.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto?> GetOrderByNumberAsync(string orderNumber);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    Task<OrderDto> CreateOrderAsync(OrderCreateDto dto);
    Task<OrderDto?> UpdateOrderAsync(OrderUpdateDto dto);
    Task<bool> DeleteOrderAsync(int id);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    Task<string> GenerateOrderNumberAsync();
}
