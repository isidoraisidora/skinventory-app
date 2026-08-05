using Domain.Dtos;
using Domain.Enums;
using Domain.Models;

namespace Service.Interface;

public interface IIngredientReactionService
{
    Task<List<IngredientReaction>> GetAllForUserAsync();
    Task<IngredientReaction> LogReactionAsync(IngredientReactionDto dto);
    Task<IngredientReaction> UpdateAsync(Guid id, ReactionType? type, int? severity, string? note);
    Task<IngredientReaction> DeleteAsync(Guid id);

    Task<List<Ingredient>> GetConflictingIngredientsAsync(Guid productId);
}