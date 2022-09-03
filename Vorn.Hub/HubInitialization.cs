using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
public static class HubInitialization
{
    public static void AddHubServer(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddSignalR().AddMessagePackProtocol();
    }
}
