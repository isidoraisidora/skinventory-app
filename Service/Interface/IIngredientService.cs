using Domain.Models;

namespace Service.Interface;

public interface IIngredientService
{
    Task<List<Ingredient>> GetAllAsync();
    Task<Ingredient> GetByIdNotNullAsync(Guid id);
    Task<Ingredient> CreateAsync(string name, string inciName);
    Task<Ingredient> UpdateAsync(Guid id, string? name, string? inciName);
    Task<Ingredient> DeleteAsync(Guid id);
}