using Microsoft.AspNetCore.Http;
using ServerSentEvents.Abstractions;
using System.Text.Json;

namespace ServerSentEvents.Processor;

public class ServerSentEventConnection : IServerSentEventConnection
{
    public Guid Guid { get; } 

    private readonly HttpResponse _response; 
    private readonly CancellationToken _cancellationToken; 

    public ServerSentEventConnection(HttpResponse response, CancellationToken cancellationToken)
    {
        Guid = Guid.NewGuid();
        _response = response; 
        _cancellationToken = cancellationToken;
    }

    public async Task<IServerSentEventResponse> SendAsync<T>(T message) where T : IServerSentEvent
    {
        try
        {
            if (_cancellationToken.IsCancellationRequested is false)
            {
                var @event = message?.GetType().Name;
                var type = message?.GetType()!;

                await _response.WriteAsync($"event: {@event}\n");
                await _response.WriteAsync($"data: ");
                await JsonSerializer.SerializeAsync(_response.Body, message, type);
                await _response.WriteAsync($"\n\n");
                await _response.Body.FlushAsync();

                return new ServerSentEventResponse(true, Guid);
            }
        }
        catch
        {
            // Client likely disconnected
        }

        return new ServerSentEventResponse(false, Guid);
    }
} 