using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mappers;

public class CategoryMapper
{
    private readonly ICategoryService _categoryService;

    public CategoryMapper(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var result = await _categoryService.GetAllAsync();
        return result.Select(x => x.ToResponse()).ToList();
    }
    
    public async Task<List<CategoryResponse>> GetForProductAsync(Guid productId)
    {
        var result = await _categoryService.GetCategoriesForProductAsync(productId);
        return result.Select(x => x.ToResponse()).ToList();
    }
    
}