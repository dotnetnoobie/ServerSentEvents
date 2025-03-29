namespace ServerSentEvents.Abstractions;

public record ServerSentEventWelcome() : IServerSentEvent
{
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    public string Message { get; init; } = "Welcome";
}
