using BLRB2B.Web.Models;

namespace BLRB2B.Web.Services;

public class CustomerService
{
    private readonly HttpService _httpService;
    private const string BasePath = "api/customers";

    public CustomerService(HttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<PagedResult<CustomerListModel>?> GetCustomersAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
    {
        var searchQuery = string.IsNullOrEmpty(search) ? "" : $"&search={Uri.EscapeDataString(search)}";
        return await _httpService.GetAsync<PagedResult<CustomerListModel>>($"{BasePath}?pageNumber={pageNumber}&pageSize={pageSize}{searchQuery}");
    }

    public async Task<CustomerDetailModel?> GetCustomerAsync(int id)
    {
        return await _httpService.GetAsync<CustomerDetailModel>($"{BasePath}/{id}");
    }

    public async Task<CustomerDetailModel?> CreateCustomerAsync(CreateCustomerModel model)
    {
        return await _httpService.PostAsync<CreateCustomerModel, CustomerDetailModel>(BasePath, model);
    }

    public async Task UpdateCustomerAsync(int id, UpdateCustomerModel model)
    {
        await _httpService.PutAsync($"{BasePath}/{id}", model);
    }

    public async Task DeleteCustomerAsync(int id)
    {
        await _httpService.DeleteAsync($"{BasePath}/{id}");
    }

    public async Task<List<CustomerDropdownModel>?> GetCustomerDropdownAsync()
    {
        return await _httpService.GetAsync<List<CustomerDropdownModel>>($"{BasePath}/dropdown");
    }
}

// Service-specific models for API requests
public class CreateCustomerModel
{
    public string CompanyName { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? CustomerGroupId { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal CreditLimit { get; set; }
    public string? ContactPerson { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}

public class UpdateCustomerModel
{
    public string CompanyName { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? CustomerGroupId { get; set; }
    public bool IsActive { get; set; }
    public decimal CreditLimit { get; set; }
    public string? ContactPerson { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}
