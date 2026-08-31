using Domain.Models;

namespace Service.Interface;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category> GetByIdNotNullAsync(Guid id);
    Task<List<Category>> GetCategoriesForProductAsync(Guid productId);
}