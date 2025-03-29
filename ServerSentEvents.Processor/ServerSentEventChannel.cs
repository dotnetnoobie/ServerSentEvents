using ServerSentEvents.Abstractions;
using System.Threading.Channels;

namespace ServerSentEvents.Processor;
 
public sealed class ServerSentEventChannel()
{
    private readonly Channel<IServerSentEvent> _queue = Channel.CreateUnbounded<IServerSentEvent>(new UnboundedChannelOptions());
    public ChannelReader<IServerSentEvent> Reader => _queue.Reader; 

    public ValueTask WriteAsync(IServerSentEvent @event, CancellationToken cancellationToken = default)
    {
        return _queue.Writer.WriteAsync(@event, cancellationToken);
    }
}
