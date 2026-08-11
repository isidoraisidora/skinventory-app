using Domain.Dtos;

namespace Web.Mappers;

using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

public class ProductMapper
{
    private readonly IProductService _productService;

    public ProductMapper(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<List<ProductResponse>> GetAllAsync(string? name, string? brand)
    {
        var result = await _productService.GetAllAsync(name, brand);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id)
    {
        var result = await _productService.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<ProductResponse> InsertAsync(ProductRequest request)
    {
        var result = await _productService.CreateAsync(
            name: request.Name,
            brand: request.Brand,
            price: request.Price ?? 0,
            description: request.Description ?? string.Empty,
            imageUrl: request.ImageUrl ?? string.Empty);

        return result.ToResponse();
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request)
    {
        var result = await _productService.UpdateAsync(
            id: id,
            name: request.Name,
            brand: request.Brand,
            price: request.Price,
            description: request.Description,
            imageUrl: request.ImageUrl);

        return result.ToResponse();
    }

    public async Task<ProductBasicResponse> DeleteAsync(Guid id)
    {
        var result = await _productService.DeleteByIdAsync(id);
        return result.ToBasicResponse();
    }

    public async Task<PaginatedResponse<ProductResponse>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _productService.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(x => x.ToResponse());
    }
}