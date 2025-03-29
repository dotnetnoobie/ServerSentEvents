using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ServerSentEvents.Abstractions;

public record ServerSentEventAggregator : IServerSentEventAggregator
{
    // original https://github.com/shiftkey/Reactive.EventAggregator 
    // based on http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/
    // and http://machadogj.com/2011/3/yet-another-event-aggregator-using-rx.html

    bool _disposed;
    readonly Subject<object> _subject = new Subject<object>();

    public IObservable<TEvent> OnEvent<TEvent>()
    {
        return _subject.OfType<TEvent>().AsObservable();
    }

    public void Publish<TEvent>(TEvent @event)
    {
        if (_subject.IsDisposed)
            return;

        _subject.OnNext(@event!);
    }

    ///// <summary>
    ///// Subscribes to events of type <typeparamref name="TEvent"/> and returns an observable for further filtering.
    ///// </summary>
    //public IObservable<TEvent> Subscribe<TEvent>(Action<TEvent> onNext)
    //{
    //    var observable = _subject.OfType<TEvent>().AsObservable();
    //    return observable.Do(onNext); // Calls onNext when an event is received
    //}

    public IDisposable Subscribe<TEvent>(Action<TEvent> onNext)
    {
        return _subject.OfType<TEvent>()
                .AsObservable()
                .Subscribe(onNext);
    }

    public void Dispose() => Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _subject.Dispose();
        _disposed = true;
    }
}