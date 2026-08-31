namespace Web.Response;

public record ProductResponse(
    Guid Id,
    string Name,
    string Brand,
    decimal? Price,
    string? Description,
    List<string> Categories
    );