using Microsoft.AspNetCore.Mvc;
using BLRB2B.Application.Interfaces;
using BLRB2B.Application.Dto;

namespace BLRB2B.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var orders = await _orderService.GetAllOrdersAsync();
        var pagedOrders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return Ok(new { data = pagedOrders, total = orders.Count(), pageNumber, pageSize });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound(new { message = $"Order with ID {id} not found" });
        }
        return Ok(order);
    }

    [HttpGet("number/{orderNumber}")]
    public async Task<IActionResult> GetByNumber(string orderNumber)
    {
        var order = await _orderService.GetOrderByNumberAsync(orderNumber);
        if (order == null)
        {
            return NotFound(new { message = $"Order with number {orderNumber} not found" });
        }
        return Ok(order);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
        return Ok(orders);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var orders = await _orderService.GetOrdersByStatusAsync(status);
        return Ok(orders);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var allOrders = await _orderService.GetAllOrdersAsync();

        var summary = new
        {
            total = allOrders.Count(),
            pending = allOrders.Count(o => o.Status == "Pending"),
            processing = allOrders.Count(o => o.Status == "Processing"),
            shipped = allOrders.Count(o => o.Status == "Shipped"),
            delivered = allOrders.Count(o => o.Status == "Delivered"),
            cancelled = allOrders.Count(o => o.Status == "Cancelled"),
            totalRevenue = allOrders.Where(o => o.Status == "Delivered").Sum(o => o.NetAmount),
            pendingRevenue = allOrders.Where(o => o.Status == "Pending" || o.Status == "Processing").Sum(o => o.NetAmount)
        };

        return Ok(summary);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (dto.OrderItems == null || !dto.OrderItems.Any())
        {
            return BadRequest(new { message = "Order must contain at least one item" });
        }

        try
        {
            var order = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != dto.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        try
        {
            var order = await _orderService.UpdateOrderAsync(dto);
            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto statusDto)
    {
        if (string.IsNullOrEmpty(statusDto.Status))
        {
            return BadRequest(new { message = "Status is required" });
        }

        var result = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
        if (!result)
        {
            return NotFound(new { message = $"Order with ID {id} not found" });
        }

        var order = await _orderService.GetOrderByIdAsync(id);
        return Ok(order);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Order with ID {id} not found" });
        }
        return Ok(new { message = "Order deleted successfully" });
    }
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}
