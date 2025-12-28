using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;
using BLRB2B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BLRB2B.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetByTaxNumberAsync(string taxNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.TaxNumber == taxNumber);
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _dbSet.Where(c => c.IsActive).ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        return excludeId == null
            ? !await _dbSet.AnyAsync(c => c.Email == email)
            : !await _dbSet.AnyAsync(c => c.Email == email && c.Id != excludeId.Value);
    }

    public async Task<bool> IsTaxNumberUniqueAsync(string taxNumber, int? excludeId = null)
    {
        return excludeId == null
            ? !await _dbSet.AnyAsync(c => c.TaxNumber == taxNumber)
            : !await _dbSet.AnyAsync(c => c.TaxNumber == taxNumber && c.Id != excludeId.Value);
    }
}
