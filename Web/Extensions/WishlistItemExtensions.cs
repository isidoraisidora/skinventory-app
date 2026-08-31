using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class WishlistItemExtensions
{
    public static WishlistItemResponse ToResponse(this WishlistItem item)
    {
        return new WishlistItemResponse(
            item.Id,
            item.ProductId,
            item.Product?.Name ?? string.Empty,
            item.CreatedAt,
            item.WishlistStatus.ToString()
        );
    }
}