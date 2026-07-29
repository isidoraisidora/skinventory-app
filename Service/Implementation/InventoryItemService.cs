using Domain.Enums;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InventoryItemService : IInventoryItemService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<InventoryItem> _inventoryItemRepository;
    private readonly IExpirationCalculator _expirationCalculator;


    public InventoryItemService(ICurrentUserService currentUserService, IRepository<InventoryItem> hasProductRepository, IExpirationCalculator expirationCalculator)
    {
        _currentUserService = currentUserService;
        _inventoryItemRepository = hasProductRepository;
        _expirationCalculator = expirationCalculator;
    }

    public async Task<List<InventoryItem>> GetAllOwnedProducts()
    {
        var user = _currentUserService.GetUserId();
        var hasProducts = await _inventoryItemRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.CreatedById == user);

        return hasProducts;
    }

    public async Task<InventoryItem> AddProductToOwned(Guid productId, string? comment, int? rating, DateTime? expirationDate, DateTime? openedDate, int? paoMonths)
    {
        var user =  _currentUserService.GetUserId();
        var existing = await _inventoryItemRepository.ExistsAsync(
            x => x.CreatedById == user && x.ProductId == productId && x.ProductStatus == ProductStatus.Opened);
        if (existing)
            throw new InvalidOperationException("Product is already in your inventory.");
        
        var hasProduct = new InventoryItem()
        {
            CreatedById = user,
            CreatedAt = DateTime.Now,
            ProductId = productId,
            Comment = comment,
            Rating = rating,
            ExpirationDate = expirationDate,
            OpenedDate = openedDate,
            PaoMonths = paoMonths,
            ProductStatus = ProductStatus.Active
        };

        return await _inventoryItemRepository.InsertAsync(hasProduct);

    }

    public async Task<InventoryItem> RemoveProductFromOwned(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var existing = await _inventoryItemRepository.ExistsAsync(
            x => x.CreatedById == user && x.ProductId == productId);
        if (!existing)
            throw new InvalidOperationException("Product doesn't exist in your inventory.");
        var hasProduct = await _inventoryItemRepository.GetAsync(
            selector: x => x,
            predicate: x => x.CreatedById == user && x.ProductId == productId);
        if (hasProduct == null) throw new Exception();
        
        return await _inventoryItemRepository.DeleteAsync(hasProduct);
    }

    public async Task<InventoryItem> UpdateProductAsync(Guid productId, string? comment, int? rating, DateTime? openedDate, ProductStatus? status,
        DateTime? expirationDate, int? paoMonths)
    {
        var user = _currentUserService.GetUserId();
        var existing = await _inventoryItemRepository.ExistsAsync(
            x => x.CreatedById == user && x.ProductId == productId);
        if (!existing)
            throw new InvalidOperationException("Product doesn't exist in your inventory.");
        
        var hasProduct = await _inventoryItemRepository.GetAsync(
            selector: x => x,
            predicate: x => x.CreatedById == user && x.ProductId == productId);
        if (hasProduct == null) throw new Exception("Product doesn't exist in your inventory.");

        hasProduct.Comment = comment;
        hasProduct.Rating = rating;
        hasProduct.OpenedDate = openedDate;
        hasProduct.ProductStatus = status;
        hasProduct.ExpirationDate = expirationDate;
        hasProduct.PaoMonths = paoMonths;

        return await _inventoryItemRepository.UpdateAsync(hasProduct);
    }
}