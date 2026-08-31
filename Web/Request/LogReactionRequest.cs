using Domain.Enums;

namespace Web.Request;

public record LogReactionRequest(Guid ProductId, Guid IngredientId, ReactionType Type, int Severity, string? Note);
