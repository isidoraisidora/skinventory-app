using Domain.Enums;
using Domain.Models;

namespace Service.Interface;

public interface IIngredientReactionService
{
    Task<List<IngredientReaction>> GetAllForUserAsync();
    Task<IngredientReaction> LogReactionAsync(Guid productId, Guid ingredientId, ReactionType type, int severity, string? note);
    Task<IngredientReaction> UpdateAsync(Guid id, ReactionType? type, int? severity, string? note);
    Task<IngredientReaction> DeleteAsync(Guid id);

    Task<List<Ingredient>> GetConflictingIngredientsAsync(Guid productId);
}