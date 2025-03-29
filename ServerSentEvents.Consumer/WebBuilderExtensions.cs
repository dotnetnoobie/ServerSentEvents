using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerSentEvents.Abstractions;
using System.Reflection;

namespace ServerSentEvents.Consumer;

public static class WebBuilderExtensions
{ 
    //public static IServiceCollection AddServerSentEventsConsumer(this WebAssemblyHostBuilder builder, Action<ServerSentEventOptions>? config = default)
    //{
    //    var assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));

    //    var types = assemblies.SelectMany(assembly => assembly.GetTypes().Where(type => typeof(IServerSentEvent).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)).ToArray();

    //    return AddServerSentEventsConsumer(builder, config, types);
    //}

    public static IServiceCollection AddServerSentEventsConsumer(this WebAssemblyHostBuilder builder, params Type[] eventTypes)
    { 
        Type[] types = [.. eventTypes, typeof(ServerSentEventHeartBeat), typeof(ServerSentEventWelcome)];

        return AddServerSentEventsConsumer(builder, null, types);
    }

    public static IServiceCollection AddServerSentEventsConsumer(this WebAssemblyHostBuilder builder, Action<ServerSentEventOptions>? config = default, params Type[] eventTypes)
    {
        builder.Services.AddHttpClient("ServerSentEventsHttpClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

        //builder.Services.ConfigureHttpClientDefaults(config =>
        //{
        //    config.ConfigureHttpClient(client =>
        //    {
        //        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        //    });
        //});

        // Type[] types = [.. eventTypes, typeof(ServerSentEventHeartBeat), typeof(ServerSentEventWelcome)];

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
