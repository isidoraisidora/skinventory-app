using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InventoryItemService : IInventoryItemService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<InventoryItem> _hasProductRepository;

    public InventoryItemService(ICurrentUserService currentUserService, IRepository<InventoryItem> hasProductRepository)
    {
        _currentUserService = currentUserService;
        _hasProductRepository = hasProductRepository;
    }

    public async Task<List<InventoryItem>> GetAllOwnedProducts()
    {
        var user = _currentUserService.GetUserId();
        var hasProducts = await _hasProductRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.UserId == user);

        return hasProducts;
    }

    public async Task<InventoryItem> AddProductToOwned(Guid productId, string? comment, int? rating, DateTime? expirationDate)
    {
        var user =  _currentUserService.GetUserId();
        var hasProduct = new InventoryItem()
        {
            UserId = user,
            ProductId = productId,
            AddedAt = DateTime.Now,
            Comment = comment,
            Rating = rating,
            ExpirationDate = expirationDate
        };

        return await _hasProductRepository.InsertAsync(hasProduct);

    }

    public async Task<InventoryItem> RemoveProductFromOwned(Guid productId)
    {
        var user = _currentUserService.GetUserId();
        var hasProduct = await _hasProductRepository.GetAsync(
            selector: x => x,
            predicate: x => x.UserId == user && x.ProductId == productId);
        if (hasProduct == null) throw new Exception();
        
        return await _hasProductRepository.DeleteAsync(hasProduct);
    }
}