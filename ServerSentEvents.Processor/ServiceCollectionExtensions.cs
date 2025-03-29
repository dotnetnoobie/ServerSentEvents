using Microsoft.Extensions.DependencyInjection;
using ServerSentEvents.Abstractions;

namespace ServerSentEvents.Processor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServerSentEvents(this IServiceCollection services, Action<ServerSentEventOptions>? config = default)
    {
        var options = new ServerSentEventOptions();
        if (config != default)
            config(options);

        services.AddScoped<IServerSentEventConsumer, ServerSentEventPreRenderClient>();
        services.AddScoped<IServerSentEventAggregator, ServerSentEventAggregator>();
        services.AddSingleton<ServerSentEventConnections>(); 
        services.AddSingleton<ServerSentEventChannel>();
        services.AddHostedService<ServerSentEventProcessor>();

        return services;
    }
}
