using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerSentEvents.Abstractions; 

namespace ServerSentEvents.Consumer;

public static class WebBuilderExtensions
{  
    public static IServiceCollection AddServerSentEventsConsumer(this WebAssemblyHostBuilder builder, params Type[] eventTypes)
    { 
        Type[] types = [.. eventTypes, typeof(ServerSentEventHeartBeat), typeof(ServerSentEventWelcome)];

        return AddServerSentEventsConsumer(builder, null, types);
    }

    public static IServiceCollection AddServerSentEventsConsumer(this WebAssemblyHostBuilder builder, Action<ServerSentEventOptions>? config = default, params Type[] eventTypes)
    {
        builder.Services.AddHttpClient("ServerSentEventsHttpClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
         
        builder.Services.AddScoped<IServerSentEventAggregator, ServerSentEventAggregator>();

        builder.Services.AddScoped<IServerSentEventConsumer>(serviceProvider =>
        {
            var options = new ServerSentEventOptions();
            if (config != default)
                config(options);

            var logger = serviceProvider.GetRequiredService<ILogger<IServerSentEventConsumer>>();
            var aggregator = serviceProvider.GetRequiredService<IServerSentEventAggregator>();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            options.Route = builder.HostEnvironment.BaseAddress + options.Route;

            var client = new ServerSentEventConsumer(logger, aggregator, options, httpClientFactory);

            client.Start(eventTypes).SafeFireAndForget(ex =>
            {
                Console.WriteLine("SafeFireAndForget: " + ex.Message);
            });

            return client;
        });

        return builder.Services;
    }
}
