namespace ServerSentEvents.Abstractions;

public interface IServerSentEventResponse
{
    Guid Guid { get; set; }
    bool Success { get; set; }
}
