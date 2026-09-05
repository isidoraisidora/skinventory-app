namespace Service.Interface;

public record EmailMessage(string ToEmail, string Subject, string Body);

public interface IEmailQueue
{
    ValueTask EnqueueAsync(EmailMessage message);
    IAsyncEnumerable<EmailMessage> DequeueAllAsync(CancellationToken cancellationToken);
}