namespace ServerSentEvents.Abstractions;

public class ServerSentEventOptions
{
    public string Route { get; set; } = "server-sent-events";
    public int MaxRetries { get; set; } = 5;
    public int HeartBeatMilliseconds { get; set; } = 5000;
    public bool SendHeartBeat { get; set; } = true;
    public string WelcomeMessage { get; set; } = "Welcome";
}
