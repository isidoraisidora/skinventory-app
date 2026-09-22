using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class IngredientExtensions
{
    public static IngredientResponse ToResponse(this Ingredient ingredient)
    {
        return new IngredientResponse(ingredient.Id, ingredient.Name, ingredient.InciName);
    }
}