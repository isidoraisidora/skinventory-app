using Domain.Models;

namespace Service.Interface;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category> GetByIdNotNullAsync(Guid id);
    Task<Category> CreateAsync(string name);
    Task<Category> UpdateAsync(Guid id, string name);
    Task<Category> DeleteAsync(Guid id);

    Task<List<Category>> GetCategoriesForProductAsync(Guid productId);
    Task TagProductAsync(Guid productId, Guid categoryId);
    Task UntagProductAsync(Guid productId, Guid categoryId);
}