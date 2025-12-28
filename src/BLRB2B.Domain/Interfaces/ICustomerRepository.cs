using BLRB2B.Domain.Entities;

namespace BLRB2B.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByTaxNumberAsync(string taxNumber);
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    Task<bool> IsTaxNumberUniqueAsync(string taxNumber, int? excludeId = null);
}
