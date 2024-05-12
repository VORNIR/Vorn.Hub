using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Vorn.Hub;

public static class HubInitialization
{
    public static void AddHubServer<TConfiguration>(this WebApplicationBuilder webApplicationBuilder, Func<TConfiguration> configureHub) where TConfiguration : HubConfiguration
    {
        TConfiguration conf = configureHub();
        Microsoft.AspNetCore.SignalR.ISignalRServerBuilder builder = webApplicationBuilder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = conf.EnableDetailedErrors;
        });
        if(conf.UseMessagePack)
            builder.AddMessagePackProtocol();
    }
}
