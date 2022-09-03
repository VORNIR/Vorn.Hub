using IdentityModel.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace Vorn.Hub;
public abstract class HubClient<T> : IAsyncDisposable where T : HubConfiguration
{
    protected HubConnection HubConnection { get; private set; }
    public HubClient(T configuration)
    {
        Uri hubUri = new(configuration.Endpoint);
        HubConnection = new HubConnectionBuilder()
            .WithUrl(hubUri, o =>
            {
                o.AccessTokenProvider = () => AccessTokenProvider(configuration);
            })
            .WithAutomaticReconnect()
            .AddMessagePackProtocol()
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddConsole();
            })
            .Build();
        HubConnection.StartAsync();
    }
    private async Task<string> AccessTokenProvider(T configuration)
    {
        using HttpClient? client = new();
        DiscoveryDocumentResponse? disco = await client.GetDiscoveryDocumentAsync(configuration.Authority);
        if(disco.IsError)
        {
            throw new Exception(disco.Error);
        }
        ClientCredentialsTokenRequest? tokenRequest = new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = configuration.ClientId,
            ClientSecret = configuration.ClientSecret,
            Scope = "hub"
        };
        TokenResponse? tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
        if(tokenResponse.IsError)
        {
            throw new Exception(tokenResponse.Error);
        }
        return tokenResponse.AccessToken;
    }
    public async ValueTask DisposeAsync()
    {
        if(HubConnection != null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}