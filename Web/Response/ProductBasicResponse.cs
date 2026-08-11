namespace Web.Response;

public record ProductBasicResponse(
    Guid Id,
    string Name,
    string Brand
    );