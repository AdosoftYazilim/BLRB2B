using Microsoft.AspNetCore.Mvc;
using BLRB2B.Application.Interfaces;
using BLRB2B.Application.Dto;

namespace BLRB2B.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var pagedCustomers = customers.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return Ok(new { data = pagedCustomers, total = customers.Count(), pageNumber, pageSize });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
        {
            return NotFound(new { message = $"Customer with ID {id} not found" });
        }
        return Ok(customer);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var customer = await _customerService.GetCustomerByEmailAsync(email);
        if (customer == null)
        {
            return NotFound(new { message = $"Customer with email {email} not found" });
        }
        return Ok(customer);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var customers = await _customerService.GetActiveCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("dropdown")]
    public async Task<IActionResult> GetDropdown()
    {
        var customers = await _customerService.GetActiveCustomersAsync();
        var dropdownData = customers.Select(c => new
        {
            id = c.Id,
            name = c.CompanyName,
            contactName = c.ContactName,
            email = c.Email
        });
        return Ok(dropdownData);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if email is unique
        var isEmailUnique = await _customerService.IsEmailUniqueAsync(dto.Email);
        if (!isEmailUnique)
        {
            return BadRequest(new { message = "Customer with this email already exists" });
        }

        // Check if tax number is unique
        if (!string.IsNullOrEmpty(dto.TaxNumber))
        {
            var isTaxNumberUnique = await _customerService.IsTaxNumberUniqueAsync(dto.TaxNumber);
            if (!isTaxNumberUnique)
            {
                return BadRequest(new { message = "Customer with this tax number already exists" });
            }
        }

        try
        {
            var customer = await _customerService.CreateCustomerAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != dto.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        // Check if email is unique (excluding current customer)
        var isEmailUnique = await _customerService.IsEmailUniqueAsync(dto.Email, dto.Id);
        if (!isEmailUnique)
        {
            return BadRequest(new { message = "Customer with this email already exists" });
        }

        // Check if tax number is unique (excluding current customer)
        if (!string.IsNullOrEmpty(dto.TaxNumber))
        {
            var isTaxNumberUnique = await _customerService.IsTaxNumberUniqueAsync(dto.TaxNumber, dto.Id);
            if (!isTaxNumberUnique)
            {
                return BadRequest(new { message = "Customer with this tax number already exists" });
            }
        }

        try
        {
            var customer = await _customerService.UpdateCustomerAsync(dto);
            if (customer == null)
            {
                return NotFound(new { message = $"Customer with ID {id} not found" });
            }
            return Ok(customer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteCustomerAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Customer with ID {id} not found" });
        }
        return Ok(new { message = "Customer deleted successfully" });
    }
}
