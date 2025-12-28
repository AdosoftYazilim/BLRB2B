namespace BLRB2B.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for managing repositories and transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
