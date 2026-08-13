using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class InventoryItemExtensions
{
    public static InventoryItemResponse ToResponse(this InventoryItem item)
    {
        return new InventoryItemResponse(
            item.Id,
            item.ProductId,
            item.Product?.Name ?? string.Empty,
            item.Comment,
            item.Rating,
            item.ExpirationDate,
            item.OpenedDate,
            item.PaoMonths,
            item.ProductStatus.ToString()
        );
    }
}