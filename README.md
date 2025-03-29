# ServerSentEvents

**A lightweight and extensible Server-Sent Events (SSE) library for .NET**  

ServerSentEvents is a C# library that simplifies working with Server-Sent Events (SSE) in .NET applications. It provides a structured way to consume, process, and manage SSE connections efficiently.  

## Features

✅ **Abstractions for SSE** – Define and implement SSE consumers and processors with ease.  
✅ **Event Processing** – Handle real-time events efficiently using the provided processor.  
✅ **Flexible & Extensible** – Customize components to fit your application's needs.  
✅ **Blazor & ASP.NET Core Support** – Seamlessly integrate with Blazor Server, ASP.NET Core APIs, and other real-time applications.  

## Packages 

This repository contains a set of C# libraries for working with Server-Sent Events (SSE). The library is split into three NuGet packages, each serving a distinct purpose:
  
| Package Name                  | Description                                                         | NuGet Link                                      |
|-------------------------------|---------------------------------------------------------------------|-------------------------------------------------|
| ServerSentEvents.Abstractions | Core interfaces and abstractions for SSE                            | [https://www.nuget.org/packages/ServerSentEvents.Abstractions](https://www.nuget.org/packages/ServerSentEvents.Abstractions) |
| ServerSentEvents.Consumer     | For consuming SSE streams within Blazor WASM project                | [https://www.nuget.org/packages/ServerSentEvents.Consumer](https://www.nuget.org/packages/ServerSentEvents.Consumer) |
| ServerSentEvents.Processor    | Processing logic for SSE data within Blazor ASP.NET Hosted project  | [https://www.nuget.org/packages/ServerSentEvents.Processor](https://www.nuget.org/packages/ServerSentEvents.Processor) |

These packages are designed to work together seamlessly, sharing a common foundation while allowing flexibility for specific use cases.

## Use Cases

This package is ideal for:

- **Real-time updates** – Stock prices, live dashboards, and monitoring systems.
- **Live notifications** – Chat applications, system alerts, and event-driven workflows.
- **Streaming data** – Logs, metrics, and continuous data streams.


## Resources

🔗 **GitHub Repository:** [https://github.com/dotnetnoobie/ServerSentEvents](https://github.com/dotnetnoobie/ServerSentEvents)  

🔗 **Server-sent events:** [https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events)  
🔗 **Using server-sent events:** [https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events)  
🔗 **HTML Server-Sent Events API:** [https://www.w3schools.com/html/html5_serversentevents.asp](https://www.w3schools.com/html/html5_serversentevents.asp)  

📖 **Documentation:** [Coming Soon] 

## Get Started


In Visual Studio Create a Blazor Web App 

| Option                    | Description   |
|---------------------------|---------------|
| Framework                 | .NET 9.0      |
| Interactivite render mode | WebAssembly   |
| Interactivity location    | Global        |

Add a .NET 9.0 class library called **`Models`** to your solution 
 

### General Project Structure

| Project          | Description                                                                             |
|------------------|-----------------------------------------------------------------------------------------|
| BlazorApp.Models | C# class library shared between the host and client apps which contains message objects |
| BlazorApp        | ASP.NET app to host the Blazor WASM client app                                          |
| BlazorApp.Client | Blazor WASM client app                                                                  |
  
  
## BlazorApp.Models Project

#### Installation

You can install the package via NuGet Package Manager or the .NET CLI. Below are the commands for each package:
```bash
dotnet add package ServerSentEvents.Abstractions
```
In a shared project add message objects, example of some simple messages, message objects must implement the **`IServerSentEvent`** interface from the **`ServerSentEvents.Abstractions`** NuGet package

Concepually message objects are DTO type objects for transporting data 

```csharp
using ServerSentEvents.Abstractions;

namespace BlazorApp.Models;

public record ClearItems() : IServerSentEvent;
public record DemoEvent(DateTime Date) : IServerSentEvent;
public record SportScore(int Team1Score, int Team2Score) : IServerSentEvent;
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) : IServerSentEvent;
```

## BlazorApp Project
To use `ServerSentEvents.Processor`, follow this example:
 
### Installation

You can install the package via NuGet Package Manager or the .NET CLI. Below are the commands for each package:
```bash
dotnet add package ServerSentEvents.Processor
```

Add to the **`program.cs`** file 

```csharp
using ServerSentEvents.Processor;
```

```csharp
builder.Services.AddHttpClient();
builder.Services.AddServerSentEvents();
```

```csharp
app.UseServerSentEvents();
```

  
## BlazorApp.Client Project
To use `ServerSentEvents.Consumer`, follow this example:

### Installation

You can install the package via NuGet Package Manager or the .NET CLI. Below are the commands for each package:
```bash
dotnet add package ServerSentEvents.Consumer
```
 
Add to the **`program.cs`** file 

```csharp
builder.AddServerSentEventsConsumer([typeof(ClearItems), typeof(SportScore), typeof(WeatherForecast), typeof(DemoEvent)]);
```

**`AddServerSentEventsConsumer([])`** takes an array of types that implement **`IServerSentEvent`** these are the messages that the **`BlazorApp.Client`** WASM app will be listerning for from the **`BlazorApp`** host project


Add to the **`MainLayout.razor`** file
```razor
@code {
    [Inject] public IServerSentEventConsumer ServerSentEventClient { get; set; } = default!;
}
```

By adding this to the **`MainLayout.razor`** file it will automatically connect and listen to the BlazorApp host project for any Server Sent Events  
