namespace Web.Request;

public record MoveToOwnedRequest(DateTime? ExpirationDate, DateTime? OpenedDate, int? PaoMonths);