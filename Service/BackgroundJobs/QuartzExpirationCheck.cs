using Microsoft.Extensions.Logging;
using Quartz;
using Service.Interface;

namespace Service.BackgroundJobs;

public class QuartzExpirationCheck : IJob
{
    private readonly IExpirationCheckService _checkService;
    private readonly ILogger<QuartzExpirationCheck> _logger;

    public QuartzExpirationCheck(IExpirationCheckService checkService, ILogger<QuartzExpirationCheck> logger)
    {
        _checkService = checkService;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _checkService.RunAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running the expiration check job.");
        }
    }
}