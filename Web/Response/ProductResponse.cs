namespace Web.Response;

public record IngredientSummary(Guid Id, string Name);

public record ProductResponse(
    Guid Id,
    string Name,
    string Brand,
    decimal? Price,
    string? Description,
    List<string> Categories,
    List<IngredientSummary> Ingredients
    );