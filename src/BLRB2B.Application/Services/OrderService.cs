using AutoMapper;
using BLRB2B.Application.Dto;
using BLRB2B.Application.Interfaces;
using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;

namespace BLRB2B.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return null;

        // Load related entities
        var orderNumber = order.OrderNumber; // Will need full entity load
        var fullOrder = await _orderRepository.GetByOrderNumberAsync(orderNumber);
        return fullOrder == null ? null : _mapper.Map<OrderDto>(fullOrder);
    }

    public async Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
        return order == null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId)
    {
        var orders = await _orderRepository.GetByCustomerAsync(customerId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
    {
        var orders = await _orderRepository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> CreateOrderAsync(OrderCreateDto dto)
    {
        // Validate customer exists
        var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID '{dto.CustomerId}' not found.");
        }

        // Validate order items
        if (dto.OrderItems == null || !dto.OrderItems.Any())
        {
            throw new InvalidOperationException("Order must have at least one item.");
        }

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            OrderNumber = await GenerateOrderNumberAsync(),
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            Notes = dto.Notes,
            DeliveryDate = dto.DeliveryDate,
            ShippingAddress = dto.ShippingAddress ?? customer.Address,
            OrderItems = new List<OrderItem>()
        };

        decimal totalAmount = 0;
        decimal discountAmount = 0;
        decimal taxAmount = 0;

        foreach (var itemDto in dto.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID '{itemDto.ProductId}' not found.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException($"Product '{product.Name}' is not active.");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}");
            }

            var lineTotal = product.Price * itemDto.Quantity;
            var lineDiscount = lineTotal * customer.DiscountRate / 100;
            var lineTax = (lineTotal - lineDiscount) * 0.20m; // 20% KDV

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                TotalPrice = lineTotal,
                Notes = itemDto.Notes
            };

            order.OrderItems.Add(orderItem);
            totalAmount += lineTotal;
            discountAmount += lineDiscount;
            taxAmount += lineTax;

            // Update stock
            product.StockQuantity -= itemDto.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        order.TotalAmount = totalAmount;
        order.DiscountAmount = discountAmount;
        order.TaxAmount = taxAmount;
        order.NetAmount = totalAmount - discountAmount + taxAmount;

        var createdOrder = await _orderRepository.AddAsync(order);
        var fullOrder = await _orderRepository.GetByOrderNumberAsync(createdOrder.OrderNumber);
        return _mapper.Map<OrderDto>(fullOrder);
    }

    public async Task<OrderDto?> UpdateOrderAsync(OrderUpdateDto dto)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(dto.Id);
        if (existingOrder == null)
        {
            return null;
        }

        existingOrder.Status = dto.Status;
        existingOrder.Notes = dto.Notes;
        existingOrder.DeliveryDate = dto.DeliveryDate;
        existingOrder.ShippingAddress = dto.ShippingAddress;
        existingOrder.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(existingOrder);

        var fullOrder = await _orderRepository.GetByOrderNumberAsync(existingOrder.OrderNumber);
        return fullOrder == null ? null : _mapper.Map<OrderDto>(fullOrder);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return false;
        }

        // Restore stock
        var fullOrder = await _orderRepository.GetByOrderNumberAsync(order.OrderNumber);
        if (fullOrder?.OrderItems != null)
        {
            foreach (var item in fullOrder.OrderItems)
            {
                if (item.Product != null)
                {
                    item.Product.StockQuantity += item.Quantity;
                    await _productRepository.UpdateAsync(item.Product);
                }
            }
        }

        await _orderRepository.DeleteAsync(order);
        return true;
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            return false;
        }

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomSuffix = new Random().Next(1000, 9999);
        return $"ORD-{datePrefix}-{randomSuffix}";
    }
}
