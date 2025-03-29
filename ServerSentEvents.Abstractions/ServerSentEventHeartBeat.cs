namespace ServerSentEvents.Abstractions;

public record ServerSentEventHeartBeat() : IServerSentEvent
{
    public DateTime Beat { get; } = DateTime.UtcNow;
}