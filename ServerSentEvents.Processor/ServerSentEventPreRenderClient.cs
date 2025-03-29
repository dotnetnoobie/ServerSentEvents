using ServerSentEvents.Abstractions;

namespace ServerSentEvents.Processor;
 
public class ServerSentEventPreRenderClient : IServerSentEventConsumer
{
    //public Task ReStart()
    //{
    //    throw new NotImplementedException();
    //}

    //public Task Start(Func<string, ReadOnlySpan<byte>, IServerSentEvent?> parser)
    //{
    //    throw new NotImplementedException("Pre Render Client Only");
    //}

    public Task Start(params Type[] eventTypes)
    {
        throw new NotImplementedException("Pre Render Client Only");
    }

    //public Task Start<TEvent>(params Type[] eventTypes) where TEvent : IServerSentEvent
    //{
    //    throw new NotImplementedException("Pre Render Client Only");
    //}

    //public Task Start<TEvent>(params TEvent[] eventTypes) where TEvent : IServerSentEvent
    //{
    //    throw new NotImplementedException();
    //}
}
