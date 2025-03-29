using Microsoft.Extensions.Hosting;
using ServerSentEvents.Abstractions;

namespace ServerSentEvents.Processor;

public class ServerSentEventProcessor : BackgroundService
{
    private readonly ServerSentEventChannel _channel;
    private readonly ServerSentEventConnections _connections;

    public ServerSentEventProcessor(ServerSentEventChannel channel, ServerSentEventConnections connections)
    {
        _channel = channel;
        _connections = connections;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IServerSentEvent entry in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            var tasks = _connections.Select(client => client.SendAsync(entry));
            var responses = await Task.WhenAll(tasks);

            foreach (var item in responses.Where(response => response.Success is false))
            {
                var connection = _connections.FirstOrDefault(connection => connection.Guid == item.Guid);
                if (connection is not null)
                { 
                    _connections.Remove(connection);
                }
            }
        }

        _connections.Clear();
    }
}
