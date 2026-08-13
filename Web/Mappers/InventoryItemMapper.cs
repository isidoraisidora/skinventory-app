using Domain.Dtos;
using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mappers;

public class InventoryItemMapper
{
    private readonly IInventoryItemService _inventoryItemService;

    public InventoryItemMapper(IInventoryItemService inventoryItemService)
    {
        _inventoryItemService = inventoryItemService;
    }

    public async Task<List<InventoryItemResponse>> GetAllAsync()
    {
        var result = await _inventoryItemService.GetAllOwnedProducts();
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<InventoryItemResponse> AddAsync(CreateInventoryItemRequest request)
    {
        var dto = new InventoryItemDto
        {
            ProductId = request.ProductId,
            Comment = request.Comment,
            Rating = request.Rating,
            ExpirationDate = request.ExpirationDate,
            OpenedDate = request.OpenedDate,
            PaoMonths = request.PaoMonths
        };

        var result = await _inventoryItemService.AddProductToOwned(dto);
        return result.ToResponse();
    }

    public async Task<InventoryItemResponse> OpenAsync(Guid productId)
    {
        var result = await _inventoryItemService.OpenProductAsync(productId);
        return result.ToResponse();
    }

    public async Task<InventoryItemResponse> FinishAsync(Guid productId)
    {
        var result = await _inventoryItemService.FinishProductAsync(productId);
        return result.ToResponse();
    }

    public async Task<InventoryItemResponse> DiscardAsync(Guid productId)
    {
        var result = await _inventoryItemService.DiscardProductAsync(productId);
        return result.ToResponse();
    }

    public async Task<InventoryItemResponse> UpdateAsync(Guid productId, UpdateInventoryItemRequest request)
    {
        var result = await _inventoryItemService.UpdateProductAsync(productId, request.Comment, request.Rating);
        return result.ToResponse();
    }
}