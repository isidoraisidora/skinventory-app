using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class WishlistItemService : IWishlistItemService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<WishlistItem> _wishlistRepository;

    public WishlistItemService(ICurrentUserService currentUserService, IRepository<WishlistItem> wishlistRepository)
    {
        _currentUserService = currentUserService;
        _wishlistRepository = wishlistRepository;
    }

    public async Task<List<WishlistItem>> GetAllWishlistProducts()
    {
        var user = _currentUserService.GetUserId();
        return await _wishlistRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.UserId == user);
    }

    public async Task<WishlistItem> AddProductToWishlist(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var wishlistProduct = new WishlistItem()
        {
            UserId = user,
            ProductId = productId,
            AddedAt = DateTime.Now
        };

        return await _wishlistRepository.InsertAsync(wishlistProduct);
    }

    public async Task<WishlistItem> RemoveProductFromWishlist(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var wishlistProduct = await _wishlistRepository.GetAsync(
            selector: x => x,
            predicate: x => x.UserId == user && x.ProductId == productId);
        if (wishlistProduct == null) throw new Exception();
        
        return await _wishlistRepository.DeleteAsync(wishlistProduct);
    }
}