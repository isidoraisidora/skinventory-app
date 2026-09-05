using Microsoft.Extensions.Logging;
using Quartz;
using Service.Interface;

namespace Service.BackgroundJobs;

public class QuartzEtlSync : IJob
{
    private readonly IEtlService _etlService;
    private readonly ILogger<QuartzEtlSync> _logger;

    public QuartzEtlSync(IEtlService etlService, ILogger<QuartzEtlSync> logger)
    {
        _etlService = etlService;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _etlService.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running the ETL sync job.");
        }
    }
}