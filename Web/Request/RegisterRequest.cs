namespace Web.Request;

public record RegisterRequest(string Email, string Username, string Password, string FirstName, string LastName);
