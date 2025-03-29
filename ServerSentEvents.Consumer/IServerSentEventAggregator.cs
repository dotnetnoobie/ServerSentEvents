namespace ServerSentEvents.Consumer;

public interface IServerSentEventAggregator
{
    IObservable<TEvent> OnEvent<TEvent>();
    void Publish<TEvent>(TEvent @event);
    IDisposable Subscribe<TEvent>(Action<TEvent> onNext);
}
