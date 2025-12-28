using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;
using BLRB2B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BLRB2B.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Include(c => c.Products)
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId)
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Include(c => c.Products)
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .ToListAsync();
    }
}
