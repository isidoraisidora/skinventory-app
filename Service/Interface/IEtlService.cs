namespace Service.Interface;

public interface IEtlService
{
    Task SyncAllAsync();
    Task EnsureCategoryTagAsync(Guid productId, string categoryTag);
}