using Domain.Dtos;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<InventoryItem> _inventoryItemRepository;
    private readonly IRepository<WishlistItem> _wishlistProductRepository;

    private readonly ICurrentUserService _currentUserService;

    public ProductService(IRepository<Product> productRepository, ICurrentUserService currentUserService, IRepository<InventoryItem> hasProductRepository, IRepository<WishlistItem> wishlistProductRepository)
    {
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _inventoryItemRepository = hasProductRepository;
        _wishlistProductRepository = wishlistProductRepository;
    }

    public async Task<Product> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"Product with id {id} not found");
        }

        return result;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _productRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id,
            include: q => q.Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.Ingredients));
    }

    public async Task<List<Product>> GetAllAsync(string? name, string? brand)
    {
        var results = await _productRepository.GetAllAsync(
            selector: x => x,
            predicate: x => (name == null || x.Name.Contains(name)) &&
                            (brand == null || x.Brand.Equals(brand)),
            include: q => q.Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.Ingredients));

        return results;
    }

    public async Task<Product> CreateAsync(string name, string brand, decimal price, string description, string imageUrl)
    {
        var product = new Product()
        {
            Name = name,
            Brand = brand,
            Price = price,
            Description = description,
            ImageUrl = imageUrl
        };

        return await _productRepository.InsertAsync(product);
    }

    public async Task<Product> UpdateAsync(Guid id, string? name, string? brand, decimal? price, string? description, string? imageUrl)
    {
        var product = await GetByIdNotNullAsync(id);

        if (name != null) product.Name = name;
        if (brand != null) product.Brand = brand;
        if (price != null) product.Price = price;
        if (description != null) product.Description = description;
        if (imageUrl != null) product.ImageUrl = imageUrl;

        return await _productRepository.UpdateAsync(product);



    }

    public async Task<Product> DeleteByIdAsync(Guid id)
    {
        var product = await GetByIdNotNullAsync(id);

        var inventoryEntries = await _inventoryItemRepository.GetAllAsync(x => x, x => x.ProductId == id);
        foreach (var entry in inventoryEntries) await _inventoryItemRepository.DeleteAsync(entry);

        var wishlistEntries = await _wishlistProductRepository.GetAllAsync(x => x, x => x.ProductId == id);
        foreach (var entry in wishlistEntries) await _wishlistProductRepository.DeleteAsync(entry);

        return await _productRepository.DeleteAsync(product);
    }

    public async Task<PaginatedResult<Product>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _productRepository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize);
    }
}