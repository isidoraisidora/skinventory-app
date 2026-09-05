using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interface;

namespace Service.BackgroundJobs;

public class EmailQueueConsumer : BackgroundService
{
    private readonly IEmailQueue _emailQueue;
    private readonly IServiceScopeFactory _scopeFactory;

    public EmailQueueConsumer(IEmailQueue emailQueue, IServiceScopeFactory scopeFactory)
    {
        _emailQueue = emailQueue;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _emailQueue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                await emailSender.SendAsync(message.ToEmail, message.Subject, message.Body);
            }
            catch (Exception)
            {
            }
        }
    }
}