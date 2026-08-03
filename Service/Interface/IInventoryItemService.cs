using Domain.Enums;
using Domain.Models;

namespace Service.Interface;

public interface IInventoryItemService
{
    Task<List<InventoryItem>> GetAllOwnedProducts();
    Task<InventoryItem> AddProductToOwned(Guid productId, string? comment, int? rating, DateTime? expirationDate, DateTime? openedDate, int? paoMonths);
    Task<InventoryItem> OpenProductAsync(Guid productId);
    Task<InventoryItem> FinishProductAsync(Guid productId);
    Task<InventoryItem> DiscardProductAsync(Guid productId);
    Task<InventoryItem> UpdateProductAsync(Guid productId, string? comment, int? rating);
}