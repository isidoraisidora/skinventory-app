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
        var category = await _categoryRepository.GetAsync(
            selector: x => x, 
            predicate: x => x.Id == id);
        
        if (category == null)
            throw new InvalidOperationException("Category not found.");

        return category;
    }

    public async Task<Category> CreateAsync(string name)
    {
        var exists = await _categoryRepository.ExistsAsync(x => x.Name == name);
        if (exists)
            throw new InvalidOperationException("A category with this name already exists.");

        var category = new Category { Name = name };
        return await _categoryRepository.InsertAsync(category);
    }

    public async Task<Category> UpdateAsync(Guid id, string name)
    {
        var category = await GetByIdNotNullAsync(id);

        if (!string.IsNullOrWhiteSpace(name))
            category.Name = name;

        return await _categoryRepository.UpdateAsync(category);
    }

    public async Task<Category> DeleteAsync(Guid id)
    {
        var category = await GetByIdNotNullAsync(id);

        var tags = await _productCategoryRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.CategoryId == id);

        foreach (var tag in tags)
            await _productCategoryRepository.DeleteAsync(tag);

        return await _categoryRepository.DeleteAsync(category);
    }

    public async Task<List<Category>> GetCategoriesForProductAsync(Guid productId)
    {
        var tags = await _productCategoryRepository.GetAllAsync(
            selector: x => x.Category,
            predicate: x => x.ProductId == productId);

        return tags;
    }

    public async Task TagProductAsync(Guid productId, Guid categoryId)
    {
        var exists = await _productCategoryRepository.ExistsAsync(
            x => x.ProductId == productId && x.CategoryId == categoryId);
        if (exists)
            throw new InvalidOperationException("Product already has this category.");

        var tag = new ProductCategory { ProductId = productId, CategoryId = categoryId };
        await _productCategoryRepository.InsertAsync(tag);
    }

    public async Task UntagProductAsync(Guid productId, Guid categoryId)
    {
        var tag = await _productCategoryRepository.GetAsync(
            selector: x => x,
            predicate: x => x.ProductId == productId && x.CategoryId == categoryId);

        if (tag == null)
            throw new InvalidOperationException("Product doesn't have this category.");

        await _productCategoryRepository.DeleteAsync(tag);
    }
}