using BLRB2B.Domain.Entities;

namespace BLRB2B.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameWithCustomerAsync(string username);
    Task<User?> GetByEmailWithCustomerAsync(string email);
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
}
