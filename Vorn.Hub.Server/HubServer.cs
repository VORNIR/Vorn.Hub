using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
public static class HubServer
{
    public static void AddHubServer<TConfiguration>(this WebApplicationBuilder webApplicationBuilder, TConfiguration conf) where TConfiguration : HubServerConfiguration
    {
        Microsoft.AspNetCore.SignalR.ISignalRServerBuilder builder = webApplicationBuilder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = conf.EnableDetailedErrors;
        });
        if(conf.UseMessagePack)
            builder.AddMessagePackProtocol();
        void configureOptions(JwtBearerOptions options)
        {
            options.Authority = conf.Authority;
            options.TokenValidationParameters.ValidTypes = ["at+jwt"];
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    Microsoft.Extensions.Primitives.StringValues accessToken = context.Request.Query["access_token"];
                    Microsoft.AspNetCore.Http.PathString path = context.HttpContext.Request.Path;
                    if(!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments(conf.HubUri)))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        }
        webApplicationBuilder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions);
    }
}
