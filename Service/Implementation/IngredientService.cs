using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class IngredientService : IIngredientService
{
    private readonly IRepository<Ingredient> _ingredientRepository;
    private readonly IRepository<IngredientReaction> _reactionRepository;

    public IngredientService(IRepository<Ingredient> ingredientRepository, IRepository<IngredientReaction> reactionRepository)
    {
        _ingredientRepository = ingredientRepository;
        _reactionRepository = reactionRepository;
    }

    public async Task<List<Ingredient>> GetAllAsync()
    {
        return await _ingredientRepository.GetAllAsync(
            selector: x => x);
    }

    public async Task<Ingredient> GetByIdNotNullAsync(Guid id)
    {
        var ingredient = await _ingredientRepository.GetAsync(
            selector: x => x, 
            predicate: x => x.Id == id);
        if (ingredient == null)
            throw new InvalidOperationException("Ingredient not found.");

        return ingredient;
    }

    public async Task<Ingredient> CreateAsync(string name, string inciName)
    {
        var exists = await _ingredientRepository.ExistsAsync(x => x.InciName == inciName);
        if (exists)
            throw new InvalidOperationException("An ingredient with this INCI name already exists.");

        var ingredient = new Ingredient { Name = name, InciName = inciName };
        return await _ingredientRepository.InsertAsync(ingredient);
    }

    public async Task<Ingredient> UpdateAsync(Guid id, string? name, string? inciName)
    {
        var ingredient = await GetByIdNotNullAsync(id);

        if (!string.IsNullOrWhiteSpace(name)) ingredient.Name = name;
        if (!string.IsNullOrWhiteSpace(inciName)) ingredient.InciName = inciName;

        return await _ingredientRepository.UpdateAsync(ingredient);
    }

    public async Task<Ingredient> DeleteAsync(Guid id)
    {
        var ingredient = await GetByIdNotNullAsync(id);

        var reactionsLogged = await _reactionRepository.ExistsAsync(x => x.IngredientId == id);
        if (reactionsLogged)
            throw new InvalidOperationException("Cannot delete an ingredient that has logged reactions. Delete the reactions first.");

        return await _ingredientRepository.DeleteAsync(ingredient);
    }
}