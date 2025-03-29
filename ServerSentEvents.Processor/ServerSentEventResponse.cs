using ServerSentEvents.Abstractions;

namespace ServerSentEvents.Processor;

public record struct ServerSentEventResponse(bool Success, Guid Guid) : IServerSentEventResponse;
