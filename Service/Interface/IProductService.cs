using Domain.Models;

namespace Service.Interface;

public interface IProductService
{
    Task<Product> GetByIdNotNullAsync(Guid id);
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> GetAllAsync(string? name, string? brand);

    Task<Product> CreateAsync(string name, string brand, decimal price, string description);
    Task<Product> UpdateAsync(Guid id, string? name, string? brand, decimal? price, string? description);
    Task<Product> DeleteByIdAsync(Guid id);

    /*Task<PaginatedResult<Product>> GetPagedAsync(int pageNumber, int pageSize);*/
}