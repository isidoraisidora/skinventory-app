namespace Web.Response;


public record InventoryItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? Comment,
    int? Rating,
    DateTime? ExpirationDate,
    DateTime? OpenedDate,
    int? PaoMonths,
    string Status
);