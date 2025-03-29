using Blazor.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServerSentEvents.Consumer;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient();

builder.Services.ConfigureHttpClientDefaults(config =>
{
    config.ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    });
});

builder.AddServerSentEventsConsumer([typeof(ClearItems), typeof(SportScore), typeof(WeatherForecast), typeof(DemoEvent)]);
// builder.AddServerSentEventsConsumer();

await builder.Build().RunAsync();
