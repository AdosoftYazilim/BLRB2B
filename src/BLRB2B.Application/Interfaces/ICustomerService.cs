using BLRB2B.Application.Dto;

namespace BLRB2B.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto?> GetCustomerByEmailAsync(string email);
    Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync();
    Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto dto);
    Task<CustomerDto?> UpdateCustomerAsync(CustomerUpdateDto dto);
    Task<bool> DeleteCustomerAsync(int id);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    Task<bool> IsTaxNumberUniqueAsync(string taxNumber, int? excludeId = null);
}
