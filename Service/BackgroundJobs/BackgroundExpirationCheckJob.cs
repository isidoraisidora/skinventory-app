using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interface;

namespace Service.BackgroundJobs;

public class BackgroundExpirationCheckJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundExpirationCheckJob(IServiceScopeFactory scopeFactory)
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
                var service = scope.ServiceProvider.GetRequiredService<IExpirationCheckService>();
                await service.RunAsync();
            }
            catch (Exception)
            {
                // swallow — don't let one bad run kill the whole background loop
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}