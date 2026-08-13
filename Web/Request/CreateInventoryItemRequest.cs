namespace Web.Request;

public record CreateInventoryItemRequest(
    Guid ProductId,
    string? Comment,
    int? Rating,
    DateTime? ExpirationDate,
    DateTime? OpenedDate,
    int? PaoMonths
);