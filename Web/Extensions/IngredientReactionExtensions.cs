using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class IngredientReactionExtensions
{
    public static IngredientReactionResponse ToResponse(this IngredientReaction reaction)
    {
        return new IngredientReactionResponse(
            reaction.Id,
            reaction.ProductId,
            reaction.IngredientId,
            reaction.Ingredient?.Name ?? string.Empty,
            reaction.ReactionType.ToString(),
            reaction.ReactionSeverity,
            reaction.Note
        );
    }
}