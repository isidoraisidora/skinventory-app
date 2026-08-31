using Domain.Dtos;
using Domain.Enums;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
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
            predicate: x => x.UserId == user,
            include: q => q.Include(x => x.Product));

        return hasProducts;
    }
    
    private async Task<InventoryItem> GetOwnedItemOrThrow(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var item = await _inventoryItemRepository.GetAsync(
            selector: x => x,
            predicate: x => x.UserId == user && x.ProductId == productId,
            include: q => q.Include(x => x.Product)
            );

        if (item == null)
            throw new InvalidOperationException("Product doesn't exist in your inventory.");

        return item;
    }

    public async Task<InventoryItem> AddProductToOwned(InventoryItemDto dto)
    {
        var user = _currentUserService.GetUserId();

        var alreadyOwned = await _inventoryItemRepository.ExistsAsync(
            x => x.UserId == user && x.ProductId == dto.ProductId &&
                 (x.ProductStatus == ProductStatus.Active || x.ProductStatus == ProductStatus.Opened));

        if (alreadyOwned)
            throw new InvalidOperationException("Product is already in your inventory.");

        var item = new InventoryItem
        {
            UserId = user,
            CreatedById = user,
            CreatedAt = DateTime.UtcNow,
            ProductId = dto.ProductId,
            Comment = dto.Comment,
            Rating = dto.Rating,
            ExpirationDate = dto.ExpirationDate,
            OpenedDate = dto.OpenedDate,
            PaoMonths = dto.PaoMonths,
            ProductStatus = ProductStatus.Active
        };

        await _inventoryItemRepository.InsertAsync(item);

        return await GetOwnedItemOrThrow(dto.ProductId); 
    }
    

    public async Task<InventoryItem> OpenProductAsync(Guid productId)
    {
        var product = await GetOwnedItemOrThrow(productId);

        if (product.ProductStatus != ProductStatus.Active)
            throw new InvalidOperationException("Product needs to be active to get opened.");

        product.ProductStatus = ProductStatus.Opened;

        return await _inventoryItemRepository.UpdateAsync(product);

    }

    public async Task<InventoryItem> FinishProductAsync(Guid productId)
    {
        var product = await GetOwnedItemOrThrow(productId);

        if (product.ProductStatus != ProductStatus.Opened)
            throw new InvalidOperationException("Product needs to be opened to finish it.");

        product.ProductStatus = ProductStatus.Finished;

        return await _inventoryItemRepository.UpdateAsync(product);
    }

    public async Task<InventoryItem> DiscardProductAsync(Guid productId)
    {
        var product = await GetOwnedItemOrThrow(productId);

        if (product.ProductStatus != ProductStatus.Active && product.ProductStatus != ProductStatus.Opened)
            throw new InvalidOperationException("Product needs to be either active or opened to be discarded.");

        product.ProductStatus = ProductStatus.Discarded;

        return await _inventoryItemRepository.UpdateAsync(product);
    }
    

    public async Task<InventoryItem> UpdateProductAsync(Guid productId, string? comment, int? rating)
    {
        var item = await GetOwnedItemOrThrow(productId);

        if (comment != null) item.Comment = comment;
        if (rating != null) item.Rating = rating;

        return await _inventoryItemRepository.UpdateAsync(item);
    }
}