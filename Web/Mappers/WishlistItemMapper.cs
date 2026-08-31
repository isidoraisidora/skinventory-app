using Domain.Dtos;
using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mappers;

public class WishlistItemMapper
{
    private readonly IWishlistItemService _wishlistItemService;

    public WishlistItemMapper(IWishlistItemService wishlistItemService)
    {
        _wishlistItemService = wishlistItemService;
    }

    public async Task<List<WishlistItemResponse>> GetAllAsync()
    {
        var result = await _wishlistItemService.GetAllWishlistProducts();
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<WishlistItemResponse> AddAsync(Guid productId)
    {
        var result = await _wishlistItemService.AddProductToWishlist(productId);
        return result.ToResponse();
    }

    public async Task<WishlistItemResponse> DiscardAsync(Guid productId)
    {
        var result = await _wishlistItemService.DiscardProductFromWishlist(productId);
        return result.ToResponse();
    }

    public async Task<InventoryItemResponse> MoveToOwnedAsync(Guid productId, MoveToOwnedRequest request)
    {
        var dto = new InventoryItemDto
        {
            ProductId = productId,
            ExpirationDate = request.ExpirationDate,
            OpenedDate = request.OpenedDate,
            PaoMonths = request.PaoMonths
        };

        var result = await _wishlistItemService.MoveToOwnedAsync(productId, dto);
        return result.ToResponse();
    }
}