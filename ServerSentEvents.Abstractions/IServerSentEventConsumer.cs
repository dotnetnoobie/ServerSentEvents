namespace ServerSentEvents.Abstractions;

public interface IServerSentEventConsumer
{ 
    Task Start(params Type[] eventTypes); 
}
