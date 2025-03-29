namespace ServerSentEvents.Abstractions;

public interface IServerSentEventConnection
{
    //CancellationToken CancellationToken { get; }
    Guid Guid { get; }
    Task<IServerSentEventResponse> SendAsync<T>(T message) where T : IServerSentEvent;
    //Task StartHeartbeat(ServerSentEventOptions options, CancellationToken cancellationToken);
    //Task KillHeartbeat();



    //Func<bool> RemoveFromConnections { get; set; }
}
