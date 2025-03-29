using Microsoft.AspNetCore.Http;
using ServerSentEvents.Abstractions;
using System.Text.Json;

namespace ServerSentEvents.Processor;

public class ServerSentEventConnection : IServerSentEventConnection
{
    public Guid Guid { get; }
    //public CancellationToken CancellationToken { get; }

    private readonly HttpResponse _response;
    //private readonly ServerSentEventChannel _channel;
    private readonly CancellationToken _cancellationToken;
    //public Func<bool> RemoveFromConnections { get; set; } = default!;

    public ServerSentEventConnection(HttpResponse response, CancellationToken cancellationToken)
    {
        Guid = Guid.NewGuid();
        _response = response;
        //_channel = channel;
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


//PeriodicTimer periodicTimer = default!;
//bool continueHeartbeat = true;

//public async Task KillHeartbeat()
//{
//    continueHeartbeat = false;
//    await Task.CompletedTask;
//   // _cancellationToken.
//}

//public async Task StartHeartbeat(ServerSentEventOptions options, CancellationToken _cancellationToken = default)
//{
//    if (options.SendHeartBeat)
//    {
//        try
//        {

//            //using var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(options.HeartBeatMilliseconds));

//            periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(options.HeartBeatMilliseconds));

//            while (await periodicTimer.WaitForNextTickAsync(_cancellationToken) && continueHeartbeat)
//            {
//                await _channel.WriteAsync(new ServerSentEventHeartBeat());
//                ////var result = await this.SendAsync(new ServerSentEventHeartBeat());
//                //if (result.Success is false)
//                //{
//                //    continueHeartbeat = false;
//                //   // this.RemoveFromConnections.Invoke();
//                //}
//            }
//        }
//        catch (OperationCanceledException)
//        {
//            // Handle cancellation, but in this case, it's triggered internally

//            periodicTimer.Dispose();
//        }
//    }
//}

//public async Task StartHeartbeat(ServerSentEventOptions options)
//{
//    if (options.SendHeartBeat)
//    {
//        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
//        var token = cancellationTokenSource.Token;

//        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(options.HeartBeatMilliseconds));

//        while (await periodicTimer.WaitForNextTickAsync(token))
//        {
//            var result = await this.SendAsync(new ServerSentEventHeartBeat());
//            if (result.Success is false)
//            {
//                await cancellationTokenSource.CancelAsync();
//                this.RemoveFromConnections.Invoke();
//            }
//            // await channel.WriteAsync(new ServerSentEventHeartBeat());
//        }
//    }
//}