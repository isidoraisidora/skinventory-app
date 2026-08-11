namespace Service.Interface;

public interface IEtlService
{
    Task SyncAllAsync();
}