namespace ServerSentEvents.Abstractions;

public interface IServerSentEventConsumer
{
    //Task Start(Func<string, ReadOnlySpan<byte>, IServerSentEvent?> parser);

    Task Start(params Type[] eventTypes);

    //Task Start<TEvent>(params TEvent[] eventTypes) where TEvent : IServerSentEvent;

    //Task ReStart();

}
