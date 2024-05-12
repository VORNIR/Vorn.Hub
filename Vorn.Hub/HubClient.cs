using IdentityModel.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Vorn.Hub;
public abstract class HubClient<TConfiguration> : IAsyncDisposable where TConfiguration : HubConfiguration
{
    protected HubConnection HubConnection { get; private set; }
    private readonly TConfiguration configuration;
    public HubClient(IOptions<TConfiguration> options)
    {
        this.configuration = options.Value;
    }
    private async Task<string> AccessTokenProvider()
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
    public virtual async Task Run()
    {
        if(HubConnection is not null)
            return;
        await Run(new Uri(configuration.Endpoint));
    }
    protected virtual async Task Run(Uri hubUri)
    {
        var builder = new HubConnectionBuilder()
        .WithUrl(hubUri, o =>
        {
            o.AccessTokenProvider = () => AccessTokenProvider();
        })
            .WithAutomaticReconnect()
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddConsole();
            });
        if(configuration.UseMessagePack)
            builder.AddMessagePackProtocol();
        HubConnection = builder.Build();
        await HubConnection.StartAsync();
    }
    public async ValueTask DisposeAsync()
    {
        if(HubConnection != null)
        {
            await HubConnection.DisposeAsync();
        }
    }
}