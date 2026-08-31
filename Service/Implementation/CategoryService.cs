using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<ProductCategory> _productCategoryRepository;

    public CategoryService(IRepository<Category> categoryRepository, IRepository<ProductCategory> productCategoryRepository)
    {
        _categoryRepository = categoryRepository;
        _productCategoryRepository = productCategoryRepository;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync(selector: x => x);
    }

    public async Task<Category> GetByIdNotNullAsync(Guid id)
    {
        var category = await _categoryRepository.GetAsync(selector: x => x, predicate: x => x.Id == id);
        if (category == null)
            throw new InvalidOperationException("Category not found.");

        return category;
    }

    public async Task<List<Category>> GetCategoriesForProductAsync(Guid productId)
    {
        return await _productCategoryRepository.GetAllAsync(
            selector: x => x.Category,
            predicate: x => x.ProductId == productId);
    }
}