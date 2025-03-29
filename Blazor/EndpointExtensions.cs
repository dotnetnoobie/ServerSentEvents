using Blazor.Models;
using ServerSentEvents.Processor;

namespace Blazor;

public static class EndpointExtensions
{
    public static WebApplication AddDemoEndpoints(this WebApplication app)
    {
        app.MapGet("/weather", async (HttpContext context, ServerSentEventChannel eventsQueue) =>
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            var message = new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(Random.Shared.Next(0, 8))),
                Random.Shared.Next(-40, 50),
                summaries[Random.Shared.Next(summaries.Length)]
            );

            await eventsQueue.WriteAsync(message);
        });

        app.MapGet("/clear-items", async (HttpContext context, ServerSentEventChannel eventsQueue) =>
        {
            var message = new ClearItems();

            await eventsQueue.WriteAsync(message);

            var de = new DemoEvent(DateTime.Now);
            await eventsQueue.WriteAsync(de);
        });

        app.MapGet("/sport", async (HttpContext context, ServerSentEventChannel eventsQueue) =>
        {
            var rnd1 = Random.Shared.Next(50, 250);
            var rnd2 = Random.Shared.Next(50, 250);
            //var user = context.User;
            var message = new SportScore(rnd1, rnd2);
            await eventsQueue.WriteAsync(message);
        }); //.RequireAuthorization();

        return app;
    }
}
