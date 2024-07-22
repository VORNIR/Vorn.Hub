![Vorn-Hub](https://github.com/VORNIR/Vorn.Hub/assets/5783101/ca40ef26-e043-4fa9-9748-6be3873b1aea)

# Vorn.Hub

Vorn.Hub represents a networking communication framework that leverages real-time SignalR hubs. It encompasses a central server and multiple clients that interact securely, utilizing Vorn.Aaas authentication protocols. The fundamental concept of this framework is the facilitation of communication through data transfer classes shared between the server and client implementations, ensuring a unified approach.

# Vorn.Hub.Server

To integrate the `Vorn.Hub.Server` package, initiate installation with the command:
```
dotnet add package Vorn.Hub.Server
```
This package equips you with a server setup function and a foundational server configuration class. Incorporate the following segment into your `appsettings.json` file:
```json
{
  ...
  "Vorn": {
    "Hub": {
      "Authority": "https://localhost:xyz",
      "HubUri": "/hub/uri",
      "EnableDetailedErrors": false,
      "UseMessagePack": true
    }
  },
...
}
```
Configure the web server using the code snippet below:
```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
HubServerConfiguration configuration = builder.Configuration.GetSection(HubServerConfiguration.Section).Get<HubServerConfiguration>();
builder.AddHubServer<HubServerConfiguration>(configuration);
WebApplication app = builder.Build();
app.MapHub<TradingHubServer>(configuration.HubUri);
app.Run();
```
With these steps, you establish a foundational framework to handle client data messages, setting the stage for developing the server's interpretive and responsive capabilities. Consider the following server class implementation:
```csharp
public class TradingHubServer : Hub<ITradingHubClientEvents>
{
    public Task Report(Price price)
    {
        Console.WriteLine(price.Value + " usd");
        return Task.CompletedTask;
    }
}

public interface IMetaTraderHubClientEvents
{
    Task NotifyChanges(List<Price> prices);
}
```
In the architecture of SignalR, the server class is derived from the `Hub<T>` class, serving as the foundational class for SignalR servers. The `ITradingHubClientEvents` interface delineates the events on the client side that the server can initiate. Furthermore, the server class's method definitions constitute the set of actions that clients are able to invoke via the communication channel.

# Vorn.Hub.Client
Client types are based on the `HubClient` abstract class, with each specific client configuration deriving from the `HubClientConfiguration` class. Here is a sample implementation:
```csharp
  class TradingHubClient : HubClient<HubClientConfiguration>
  {
      public TradingHubClient(HubClientConfiguration configuration) : base(configuration)
      {
      }
      public Task Report(Price price) => HubConnection.InvokeAsync(nameof(Report), price);
  }
```
When invoking `HubConnection.InvokeAsync`, the first argument is the server function's name, and subsequent arguments are the parameters for that function.
Example usage of client side:
```csharp
  static TradingHubClient hub;
  static HubClientConfiguration configuration = new HubClientConfiguration
  {
      Authority= "https://localhost:xyz",
      ClientId="Vorn.Hub.ClientId",
      ClientSecret="VornHubSecret",
      Endpoint="https://localhost:abc/hub/uri"
  };
  public static void StartConnection()
  {

      hub = new TradingHubClient(configuration);
      hub.Run().Wait();
  }
  public static void Report(Price price)
  {
      hub.Report(price).Wait();
  }
```
# Common Classes

In the given example, the 'Price' class may be established within a shared class that is utilized by both the server and client implementations. This approach facilitates a unified structure that streamlines the integration process across different components of the system.

Note: Although the client implementations utilize .NET Standard 2.0, which is considered somewhat outdated, this choice offers greater portability across various platforms.
