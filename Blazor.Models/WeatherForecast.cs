using ServerSentEvents.Abstractions;

namespace Blazor.Models;
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) : IServerSentEvent;
