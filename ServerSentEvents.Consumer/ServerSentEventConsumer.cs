using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Logging;
using Polly;
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

    // private bool _running = false;

    public ServerSentEventConsumer(ILogger<IServerSentEventConsumer> logger, IServerSentEventAggregator eventAggregator, ServerSentEventOptions serverSentEventOptions, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _eventAggregator = eventAggregator;
        _serverSentEventOptions = serverSentEventOptions;
        _httpClientFactory = httpClientFactory;
    }

    //public Task Start<TEvent>(params TEvent[] eventTypes) where TEvent : IServerSentEvent
    //{
    //    Type[] types = eventTypes.Select(x => x.GetType()).ToArray() ?? []  ;

    //    _parser = CreateParser(types);
    //    return ParseEvents();
    //}

    //public Task Start(Func<string, ReadOnlySpan<byte>, IServerSentEvent?> parser)
    //{
    //    _parser = parser;
    //    return ParseEvents();
    //}

    //public Task Start<TEvent>(params Type[] eventTypes) where TEvent : IServerSentEvent
    //{
    //    _parser = CreateParser(eventTypes);
    //    return ParseEvents();
    //}

    public Task Start(params Type[] eventTypes)
    {
        _parser = CreateParser(eventTypes);
        return ParseEvents();
    }

    private async Task ParseEvents()
    {
        try
        {
            var message = new HttpRequestMessage(HttpMethod.Get, _serverSentEventOptions.Route);
            message.SetBrowserResponseStreamingEnabled(true);

            using var client = _httpClientFactory.CreateClient("ServerSentEventsHttpClient");

            client.DefaultRequestHeaders.Add("Accept", "text/event-stream");

            //var policy = Policy.Handle<HttpRequestException>()
            //    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            //var response = await policy.ExecuteAsync(() => client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead));

            var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

            using var stream = await response.Content.ReadAsStreamAsync();

            var readerAsync = SseParser.Create(stream, (eventType, bytes) => _parser(eventType, bytes)).EnumerateAsync();
            await foreach (var item in readerAsync)
            {
                if (item.EventType is not null && item.Data is not null)
                    _eventAggregator.Publish(item.Data);
            }

            //await foreach (var item in SseParser.Create<IServerSentEvent?>(stream, (eventType, bytes) => _parser(eventType, bytes)).EnumerateAsync())
            //{
            //    if (item.EventType is not null && item.Data is not null)
            //        _eventAggregator.Publish(item.Data);
            //}

        }
        catch (HttpRequestException ex)
        {
            if (IsConnectionResetException(ex))
            {
                // Handle connection reset during HTTP request
                Console.WriteLine("Connection reset during HTTP request.");
            }
            else
            {
                // Handle other HTTP request exceptions
                Console.WriteLine("HTTP request failed: xxxxx" + ex.Message);
            }
        }
        catch (IOException ex)
        {
            if (IsConnectionResetException(ex))
            {
                // Handle connection reset while reading stream
                Console.WriteLine("Connection reset while reading stream.");
            }
            else
            {
                // Handle other IO exceptions
                Console.WriteLine("Stream reading failed: " + ex.Message);
            }
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            Console.WriteLine("An unexpected error occurred: " + ex.Message);
        }
    }

    private bool IsConnectionResetException(Exception ex)
    {
        while (ex != null)
        {
            if (ex is System.Net.Sockets.SocketException && ex.Message.Contains("connection reset", StringComparison.OrdinalIgnoreCase))
                return true;
            if (ex is System.IO.IOException && ex.Message.Contains("connection reset", StringComparison.OrdinalIgnoreCase))
                return true;
            ex = ex.InnerException;
        }
        return false;
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




//private async Task ParseEvents(Func<string, ReadOnlySpan<byte>, IServerSentEvent?> parser)
//{
//    var message = new HttpRequestMessage(HttpMethod.Get, _serverSentEventOptions.Route);
//    message.SetBrowserResponseStreamingEnabled(true);

//    using var client = new HttpClient();
//    client.DefaultRequestHeaders.Add("Accept", "text/event-stream");

//    var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

//    using var stream = await response.Content.ReadAsStreamAsync();

//    await foreach (var item in SseParser.Create<IServerSentEvent?>(stream, (eventType, bytes) => parser(eventType, bytes)).EnumerateAsync())
//    {
//        if (item.EventType is not null && item.Data is not null)
//            _eventAggregator.Publish(item.Data);
//    }
//}



//private Func<string, ReadOnlySpan<byte>, IServerSentEvent?> CreateParser(params IServerSentEvent[] eventTypes)
//{
//    var eventTypeMap = new Dictionary<string, IServerSentEvent>();

//    foreach (var type in eventTypes)
//    {
//        //if (!typeof(IServerSentEvent).IsAssignableFrom(type))
//        //{
//        //    throw new ArgumentException($"Type {type.Name} does not implement IServerSentEvent.");
//        //}

//        eventTypeMap[type.GetType().Name] = type;
//    }

//    return (eventType, bytes) =>
//    {
//        if (eventTypeMap.TryGetValue(eventType, out var type))
//        {
//            return JsonSerializer.Deserialize(bytes, type.GetType()) as IServerSentEvent;
//        }

//        return null;
//    };
//}

