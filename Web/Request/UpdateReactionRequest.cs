using Domain.Enums;

namespace Web.Request;

public record UpdateReactionRequest(ReactionType? Type, int? Severity, string? Note);