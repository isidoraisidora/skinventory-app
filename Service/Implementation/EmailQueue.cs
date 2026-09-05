using System.Threading.Channels;
using Service.Interface;

namespace Service.Implementation;

public class EmailQueue : IEmailQueue
{
    private readonly Channel<EmailMessage> _channel = Channel.CreateUnbounded<EmailMessage>();

    public async ValueTask EnqueueAsync(EmailMessage message) =>
        await _channel.Writer.WriteAsync(message);

    public IAsyncEnumerable<EmailMessage> DequeueAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}