using Domain.Dtos;
using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class ProductExtensions
{
    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Brand,
            product.Price,
            product.Description,
            product.ProductCategories?.Select(pc => pc.Category.Name).ToList() ?? new List<string>()
        );
    }
    
    public static ProductBasicResponse ToBasicResponse(this Product product)
    {
        return new ProductBasicResponse(
            product.Id,
            product.Name,
            product.Brand
        );
    }
    
    public static PaginatedResponse<TResult> ToPaginatedResponse<T, TResult>(
        this PaginatedResult<T> result,
        Func<T, TResult> mappingFunction)
    {
        return new PaginatedResponse<TResult>
        {
            Items = result.Items.Select(mappingFunction).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }
}