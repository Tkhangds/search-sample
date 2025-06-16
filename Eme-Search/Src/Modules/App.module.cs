using Eme_Search.Modules.Blacklist;
using Eme_Search.Modules.Search;
using Eme_Search.Utils.RequestClient;

namespace Eme_Search.Modules;

public static class AppModule
{
    public static IServiceCollection AddAppDependency(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Inject Configuration
        services
            .AddSingleton<IConfiguration>(builder.Configuration);
        
        services
            .AddTransient<IRequestClient, RequestClient>();
        
        // Inject Search Module
        services.AddSearchModule();
        
        // Inject Blacklist Module
        services.AddBlacklistModule();
        
        return services;
    }
}