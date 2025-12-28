using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;
using BLRB2B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BLRB2B.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameWithCustomerAsync(string username)
    {
        return await _dbSet
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailWithCustomerAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null)
    {
        return excludeId == null
            ? !await _dbSet.AnyAsync(u => u.Username == username)
            : !await _dbSet.AnyAsync(u => u.Username == username && u.Id != excludeId.Value);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        return excludeId == null
            ? !await _dbSet.AnyAsync(u => u.Email == email)
            : !await _dbSet.AnyAsync(u => u.Email == email && u.Id != excludeId.Value);
    }
}
