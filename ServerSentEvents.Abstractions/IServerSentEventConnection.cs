namespace ServerSentEvents.Abstractions;

public interface IServerSentEventConnection
{  
    Guid Guid { get; }
    Task<IServerSentEventResponse> SendAsync<T>(T message) where T : IServerSentEvent; 
}
