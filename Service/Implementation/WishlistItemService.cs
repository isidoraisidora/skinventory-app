using Domain.Enums;
using Domain.Models;
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
            predicate: x => x.CreatedById == user);
    }
    
    private async Task<WishlistItem> GetWishlistItemOrThrow(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var item = await _wishlistRepository.GetAsync(
            selector: x => x,
            predicate: x => x.CreatedById == user && x.ProductId == productId && x.WishlistStatus == WishlistStatus.Active);

        if (item == null)
            throw new InvalidOperationException("Product doesn't exist in your wishlist.");

        return item;
    }

    public async Task<WishlistItem> AddProductToWishlist(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var existing = await _wishlistRepository.ExistsAsync(
            x => x.CreatedById == user && x.ProductId == productId && x.WishlistStatus==WishlistStatus.Active);
        if (existing)
            throw new InvalidOperationException("Product already exists in your wishlist.");
        var wishlistProduct = new WishlistItem()
        {
            UserId = user,
            CreatedById = user,
            ProductId = productId,
            CreatedAt = DateTime.UtcNow,
            WishlistStatus = WishlistStatus.Active
        };

        return await _wishlistRepository.InsertAsync(wishlistProduct);
    }

    public async Task<WishlistItem> DiscardProductFromWishlist(Guid productId)
    {
        var item = await GetWishlistItemOrThrow(productId);
        item.WishlistStatus = WishlistStatus.Discarded;
        return await _wishlistRepository.UpdateAsync(item);
    }
    
    public async Task<InventoryItem> MoveToOwnedAsync(Guid productId, DateTime? expirationDate, DateTime? openedDate, int? paoMonths)
    {
        var wishlistItem = await GetWishlistItemOrThrow(productId);

        var inventoryItem = await _inventoryItemService.AddProductToOwned(
            productId, comment: null, rating: null, expirationDate, openedDate, paoMonths);

        wishlistItem.WishlistStatus = WishlistStatus.Discarded;
        await _wishlistRepository.UpdateAsync(wishlistItem);

        return inventoryItem;
    }
}