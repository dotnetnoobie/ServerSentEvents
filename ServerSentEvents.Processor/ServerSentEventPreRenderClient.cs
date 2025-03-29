using ServerSentEvents.Abstractions;

namespace ServerSentEvents.Processor;
 
public class ServerSentEventPreRenderClient : IServerSentEventConsumer
{ 
    public Task Start(params Type[] eventTypes)
    {
        throw new NotImplementedException("Pre Render Client Only");
    } 
}
