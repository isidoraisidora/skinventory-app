namespace Web.Response;

public record WishlistItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    DateTime CreatedAt,
    string Status
);