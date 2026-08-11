namespace Web.Request;

public record ProductRequest(
    string Name,
    string Brand,
    decimal? Price,
    string? Description,
    string? ImageUrl
    );