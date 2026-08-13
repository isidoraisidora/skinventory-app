using Domain.Models;

namespace Service.Interface;

public interface IAuthService
{
    Task<User> RegisterAsync(string email, string username, string password, string firstName, string lastName);
    Task<User?> ValidateCredentialsAsync(string email, string password);
}