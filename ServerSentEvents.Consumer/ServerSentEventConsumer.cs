using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Logging;
using ServerSentEvents.Abstractions;
using System.Net.ServerSentEvents;
using System.Text.Json;

namespace ServerSentEvents.Consumer;

public class ServerSentEventConsumer : IServerSentEventConsumer
{
    private readonly ILogger<IServerSentEventConsumer> _logger;
    private readonly IServerSentEventAggregator _eventAggregator;
    private readonly ServerSentEventOptions _serverSentEventOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private Func<string, ReadOnlySpan<byte>, IServerSentEvent?> _parser = default!;

    public ServerSentEventConsumer(ILogger<IServerSentEventConsumer> logger, IServerSentEventAggregator eventAggregator, ServerSentEventOptions serverSentEventOptions, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _eventAggregator = eventAggregator;
        _serverSentEventOptions = serverSentEventOptions;
        _httpClientFactory = httpClientFactory;
    }

    public Task Start(params Type[] eventTypes)
    {
        _parser = CreateParser(eventTypes);
        return ParseEvents();
    }

    private async Task ParseEvents()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, _serverSentEventOptions.Route);
        message.SetBrowserResponseStreamingEnabled(true);

        using var client = _httpClientFactory.CreateClient("ServerSentEventsHttpClient");

        client.DefaultRequestHeaders.Add("Accept", "text/event-stream");

        var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

        using var stream = await response.Content.ReadAsStreamAsync();

        var readerAsync = SseParser.Create(stream, (eventType, bytes) => _parser(eventType, bytes)).EnumerateAsync();
        await foreach (var item in readerAsync)
        {
            if (item.EventType is not null && item.Data is not null)
                _eventAggregator.Publish(item.Data);
        }
    }

    private Func<string, ReadOnlySpan<byte>, IServerSentEvent?> CreateParser(params Type[] eventTypes)
    {
        var eventTypeMap = new Dictionary<string, Type>();

        foreach (var type in eventTypes)
        {
            if (!typeof(IServerSentEvent).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type {type.Name} does not implement IServerSentEvent.");
            }

            eventTypeMap[type.Name] = type;
        }

        return (eventType, bytes) =>
        {
            if (eventTypeMap.TryGetValue(eventType, out var type))
            {
                return JsonSerializer.Deserialize(bytes, type) as IServerSentEvent;
            }

            return null;
        };
    }
}

