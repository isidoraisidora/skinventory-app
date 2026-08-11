using Domain.Models;

namespace Service.Interface;

public interface IExternalProductApi
{
    Task<List<Product>> SearchProductsAsync(string categoryTag, int page, int pageSize);
}