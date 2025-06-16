using Eme_Search.Modules.Blacklist.Profiles;
using Eme_Search.Modules.Blacklist.Services;

namespace Eme_Search.Modules.Blacklist;

public static class BlacklistModule
{
    public static IServiceCollection AddBlacklistDependency(this IServiceCollection services)
    {
        services.AddScoped<IBlacklistService, BlacklistService>();
        
        return services;
    }
    
    public static IServiceCollection AddBlacklistMapping(this IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(BlacklistBusinessResponseProfile))
            .AddAutoMapper(typeof(BlacklistCategoryResponseProfile));

        return services;
    }
    
    public static IServiceCollection AddBlacklistModule(this IServiceCollection services)
    {
        services
            .AddBlacklistDependency()
            .AddBlacklistMapping();
        
        return services;
    }
}