using ServerSentEvents.Abstractions;

namespace Blazor.Models;

public record DemoEvent(DateTime Date) : IServerSentEvent;