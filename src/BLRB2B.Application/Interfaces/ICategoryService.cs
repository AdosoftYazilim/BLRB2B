using BLRB2B.Application.Dto;

namespace BLRB2B.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto?> GetCategoryByCodeAsync(string code);
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentCategoryId);
    Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto dto);
    Task<CategoryDto?> UpdateCategoryAsync(CategoryUpdateDto dto);
    Task<bool> DeleteCategoryAsync(int id);
}
