using ServerSentEvents.Abstractions;

namespace Blazor.Models;

public record SportScore(int Team1Score, int Team2Score) : IServerSentEvent;
