namespace Web.Response;

public record IngredientReactionResponse(
    Guid Id,
    Guid ProductId,
    Guid IngredientId,
    string IngredientName,
    string Type,
    int Severity,
    string? Note
);