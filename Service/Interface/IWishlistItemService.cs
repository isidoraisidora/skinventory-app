using Domain.Models;

namespace Service.Interface;

public interface IWishlistItemService
{
    Task<List<WishlistItem>> GetAllWishlistProducts();
    Task<WishlistItem> AddProductToWishlist(Guid productId);
    Task<WishlistItem> RemoveProductFromWishlist(Guid productId);

}