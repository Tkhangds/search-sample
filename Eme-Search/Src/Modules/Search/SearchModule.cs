using Eme_Search.Modules.Search.Services;

namespace Eme_Search.Modules.Search;

public static class SearchModule
{
    public static IServiceCollection AddSearchDependency(this IServiceCollection services)
    {
        services.AddScoped<ISearchService, YelpSearchService>();
        services.AddScoped<ISearchService, GGSearchService>();
        
        services.AddScoped<SearchServiceResolver>();
        
        return services;
    }

    public static IServiceCollection AddSearchModule(this IServiceCollection services)
    {
        services
            .AddSearchDependency();
        return services;
    }
}