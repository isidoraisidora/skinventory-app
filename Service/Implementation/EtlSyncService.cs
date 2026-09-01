using Domain.Config;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EtlSyncService : IEtlService
{
    private const string JobName = "ProductsSync";

    private readonly IRepository<EtlSyncLog> _etlSyncLogRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<Ingredient> _ingredientRepository;

    private readonly IExternalProductApi _externalProductApi;
    private readonly ProductEtlOptions _options;

    public EtlSyncService(
        IRepository<EtlSyncLog> etlSyncLogRepository,
        IRepository<Product> productRepository,
        IExternalProductApi externalProductApi,
        IOptions<ProductEtlOptions> options, IRepository<Category> categoryRepository, IRepository<ProductCategory> productCategoryRepository, IRepository<Ingredient> ingredientRepository)
    {
        _etlSyncLogRepository = etlSyncLogRepository;
        _productRepository = productRepository;
        _externalProductApi = externalProductApi;
        _categoryRepository = categoryRepository;
        _productCategoryRepository = productCategoryRepository;
        _ingredientRepository = ingredientRepository;
        _options = options.Value;
    }
    
    public async Task EnsureCategoryTagAsync(Guid productId, string categoryTag)
    {
        var displayName = CategoryTagMapper.ToDisplayName(categoryTag);

        var category = await _categoryRepository.GetAsync(x => x, x => x.Name == displayName);
        if (category == null)
        {
            category = await _categoryRepository.InsertAsync(new Category { Name = displayName });
        }

        var alreadyTagged = await _productCategoryRepository.ExistsAsync(
            x => x.ProductId == productId && x.CategoryId == category.Id);

        if (!alreadyTagged)
        {
            await _productCategoryRepository.InsertAsync(new ProductCategory
            {
                ProductId = productId,
                CategoryId = category.Id
            });
        }
    }
    
    private async Task LinkIngredientsAsync(Guid productId, List<string> ingredientNames)
    {
        if (ingredientNames.Count == 0)
            return;

        var product = await _productRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == productId,
            include: q => q.Include(p => p.Ingredients));

        if (product == null)
            return;

        foreach (var name in ingredientNames)
        {
            var ingredient = await _ingredientRepository.GetAsync(x => x, x => x.Name == name);
            if (ingredient == null)
            {
                ingredient = await _ingredientRepository.InsertAsync(new Ingredient { Name = name, InciName = name });
            }

            if (product.Ingredients.All(i => i.Id != ingredient.Id))
            {
                product.Ingredients.Add(ingredient);
            }
        }

        product.Description = null; 
        await _productRepository.UpdateAsync(product);
    }

    public async Task SyncAllAsync()
    {
        var log = new EtlSyncLog { JobName = JobName, StartedAt = DateTime.UtcNow };
        var imported = 0;
        var updated = 0;
        var skipped = 0;
        var pagesFailed = 0;

        try
        {
            foreach (var category in _options.CategoryTags)
            {
                for (var page = 1; page <= _options.MaxPages; page++)
                {
                    List<Product> products;

                    try
                    {
                        products = await _externalProductApi.SearchProductsAsync(category, page, _options.PageSize);
                    }
                    catch (Exception)
                    {
                        pagesFailed++;
                        continue; 
                    }

                    if (products.Count == 0)
                        break; 

                    foreach (var product in products)
                    {
                        var existing = await _productRepository.GetAsync(x => x, x => x.Barcode == product.Barcode);

                        Product savedProduct;

                        if (existing != null)
                        {
                            existing.Name = product.Name;
                            existing.Brand = product.Brand;
                            existing.Description = product.Description; 
                            await _productRepository.UpdateAsync(existing);
                            savedProduct = existing;
                            updated++;
                        }
                        else
                        {
                            savedProduct = await _productRepository.InsertAsync(product);
                            imported++;
                        }

                        await EnsureCategoryTagAsync(savedProduct.Id, category);

                        var ingredientNames = ExternalProductTransformer.ParseIngredientNames(product.Description);
                        await LinkIngredientsAsync(savedProduct.Id, ingredientNames);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(7));
                }
            }

            log.Success = true;
            log.ProductsImported = imported;
            log.ProductsUpdated = updated;
            log.ProductsSkipped = skipped;
        }
        catch (Exception ex)
        {
            log.Success = false;
            log.ErrorMessage = ex.Message;
        }
        finally
        {
            log.CompletedAt = DateTime.UtcNow;
            await _etlSyncLogRepository.InsertAsync(log);
        }
    }
}