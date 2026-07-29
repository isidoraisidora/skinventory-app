using Domain.Enums;
using Domain.Models;

namespace Service.Interface;

public interface IInventoryItemService
{
    Task<List<InventoryItem>> GetAllOwnedProducts();
    Task<InventoryItem> AddProductToOwned(Guid productId, string? comment, int? rating, DateTime? expirationDate, DateTime? openedDate, int? paoMonths);
    Task<InventoryItem> RemoveProductFromOwned(Guid productId);
    Task<InventoryItem> UpdateProductAsync(Guid productId, string? comment, int? rating, DateTime? openedDate, ProductStatus? status, DateTime? expirationDate, int? paoMonths);
}