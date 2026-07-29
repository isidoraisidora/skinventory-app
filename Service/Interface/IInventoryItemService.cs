using Domain.Models;

namespace Service.Interface;

public interface IInventoryItemService
{
    Task<List<InventoryItem>> GetAllOwnedProducts();
    Task<InventoryItem> AddProductToOwned(Guid productId, string? comment, int? rating, DateTime? expirationDate);
    Task<InventoryItem> RemoveProductFromOwned(Guid productId);
}