using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using ServerSentEvents.Abstractions;
using System.Text.Json;

namespace ServerSentEvents.Processor;

public static class WebApplicationCollectionExtensions
{
    public static WebApplication UseServerSentEvents(this WebApplication app, Action<ServerSentEventOptions>? config = default)
    {
        var options = new ServerSentEventOptions();

        if (config != default)
            config(options);

        app.MapGet(options.Route, async (HttpContext context, ServerSentEventConnections connections, CancellationToken cancellationToken = default) =>
        {
            var response = context.Response;
            var message = new ServerSentEventWelcome { Message = $"{context.User?.Identity?.Name ?? "Guest"} - {options.WelcomeMessage}" };

            response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
            await response.WriteAsync($"event: {nameof(ServerSentEventWelcome)}\n");
            await response.WriteAsync($"data: ");
            await JsonSerializer.SerializeAsync(response.Body, message, typeof(ServerSentEventWelcome));
            await response.WriteAsync($"\n\n");
            await response.Body.FlushAsync();

            var connection = new ServerSentEventConnection(response, context.RequestAborted);
            //connection.RemoveFromConnections = () => connections.Remove(connection);
            connections.Add(connection);
            //connection.StartHeartbeat(options, cancellationToken).SafeFireAndForget();

            try
            {
                while (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(options.HeartBeatMilliseconds);
                    // await channel.WriteAsync(new ServerSentEventHeartBeat());
                }
            }
            catch (OperationCanceledException)
            {
                // Client disconnected, no problem 
                // connections.Remove(connection);

                Console.WriteLine($"Client disconnected: {connection.Guid}");
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                connections.Remove(connection);
            }
        });

        return app;
    }
}