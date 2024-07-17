using IdentityModel.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vorn.Hub
{
    public abstract class HubClient<TConfiguration> : IAsyncDisposable where TConfiguration : HubClientConfiguration
    {
        protected HubConnection HubConnection { get; private set; }
        private readonly TConfiguration configuration;
        public HubClient(IOptions<TConfiguration> options)
        {
            this.configuration = options.Value;
        }
        public HubClient(TConfiguration configuration)
        {
            this.configuration = configuration;
        }
        private async Task<string> AccessTokenProvider()
        {
            using(HttpClient client = new HttpClient())
            {
                DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(configuration.Authority);
                if(disco.IsError)
                {
                    throw new Exception(disco.Error);
                }
                ClientCredentialsTokenRequest tokenRequest = new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = configuration.ClientId,
                    ClientSecret = configuration.ClientSecret
                };
                TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
                if(tokenResponse.IsError)
                {
                    throw new Exception(tokenResponse.Error);
                }
                return tokenResponse.AccessToken;
            }
        }
        public virtual async Task Run()
        {
            if(HubConnection != null)
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
            .WithAutomaticReconnect();
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
}