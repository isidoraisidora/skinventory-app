using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interface;

namespace Service.BackgroundJobs;

public class BackgroundEtlSyncJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundEtlSyncJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var etlService = scope.ServiceProvider.GetRequiredService<IEtlService>();
                await etlService.SyncAllAsync();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Unsuccessful sync.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}