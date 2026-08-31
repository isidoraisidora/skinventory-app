using Domain.Dtos;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class WishlistItemService : IWishlistItemService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<WishlistItem> _wishlistRepository;
    private readonly IInventoryItemService _inventoryItemService;

    public WishlistItemService(ICurrentUserService currentUserService, IRepository<WishlistItem> wishlistRepository, IInventoryItemService inventoryItemService)
    {
        _currentUserService = currentUserService;
        _wishlistRepository = wishlistRepository;
        _inventoryItemService = inventoryItemService;
    }

    public async Task<List<WishlistItem>> GetAllWishlistProducts()
    {
        var user = _currentUserService.GetUserId();
        return await _wishlistRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.UserId == user,
            include: q => q.Include(x => x.Product));
    }
    
    private async Task<WishlistItem> GetWishlistItemOrThrow(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var item = await _wishlistRepository.GetAsync(
            selector: x => x,
            predicate: x => x.UserId == user && x.ProductId == productId && x.WishlistStatus == WishlistStatus.Active,
            include: q => q.Include(x => x.Product));

        if (item == null)
            throw new InvalidOperationException("Product doesn't exist in your wishlist.");

        return item;
    }

    public async Task<WishlistItem> AddProductToWishlist(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var existing = await _wishlistRepository.ExistsAsync(
            x => x.UserId == user && x.ProductId == productId && x.WishlistStatus == WishlistStatus.Active);
        if (existing)
            throw new InvalidOperationException("Product already exists in your wishlist.");

        var wishlistProduct = new WishlistItem
        {
            UserId = user,
            CreatedById = user,
            ProductId = productId,
            CreatedAt = DateTime.UtcNow,
            WishlistStatus = WishlistStatus.Active
        };

        await _wishlistRepository.InsertAsync(wishlistProduct);

        return await GetWishlistItemOrThrow(productId);
    }

    public async Task<WishlistItem> DiscardProductFromWishlist(Guid productId)
    {
        var item = await GetWishlistItemOrThrow(productId);
        item.WishlistStatus = WishlistStatus.Discarded;
        return await _wishlistRepository.UpdateAsync(item);
    }
    
    public async Task<InventoryItem> MoveToOwnedAsync(Guid productId, InventoryItemDto dto)
    {
        var wishlistItem = await GetWishlistItemOrThrow(productId);

        var inventoryItem = await _inventoryItemService.AddProductToOwned(dto);

        wishlistItem.WishlistStatus = WishlistStatus.Discarded;
        await _wishlistRepository.UpdateAsync(wishlistItem);

        return inventoryItem;
    }
}