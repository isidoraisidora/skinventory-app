using Domain.Config;
using Domain.Models;
using Microsoft.Extensions.Options;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EtlSyncService : IEtlService
{
   private const string JobName = "ProductsSync";

    private readonly IRepository<EtlSyncLog> _etlSyncLogRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IExternalProductApi _externalProductApi;
    private readonly ProductEtlOptions _options;

    public EtlSyncService(
        IRepository<EtlSyncLog> etlSyncLogRepository,
        IRepository<Product> productRepository,
        IExternalProductApi externalProductApi,
        IOptions<ProductEtlOptions> options)
    {
        _etlSyncLogRepository = etlSyncLogRepository;
        _productRepository = productRepository;
        _externalProductApi = externalProductApi;
        _options = options.Value;
    }

    public async Task SyncAllAsync()
    {
        var log = new EtlSyncLog
        {
            JobName = JobName,
            StartedAt = DateTime.UtcNow
        };

        var imported = 0;
        var updated = 0;
        var skipped = 0;

        try
        {
            for (var page = 1; page <= _options.MaxPages; page++)
            {
                // EXTRACT
                var products = await _externalProductApi.SearchProductsAsync(
                    _options.CategoryTag, page, _options.PageSize);

                if (products.Count == 0)
                    break; // no more results, stop paging early

                foreach (var product in products)
                {
                    // TRANSFORM already happened inside SearchProductsAsync (via the mapper)

                    // LOAD — upsert by barcode
                    var existing = await _productRepository.GetAsync(
                        selector: x => x,
                        predicate: x => x.Barcode == product.Barcode);

                    if (existing != null)
                    {
                        existing.Name = product.Name;
                        existing.Brand = product.Brand;
                        existing.Description = product.Description;
                        await _productRepository.UpdateAsync(existing);
                        updated++;
                    }
                    else
                    {
                        await _productRepository.InsertAsync(product);
                        imported++;
                    }
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