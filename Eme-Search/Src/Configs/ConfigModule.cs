using Eme_Search.Modules;

using FluentValidation;
using FluentValidation.AspNetCore;

namespace Eme_Search.Configs;

public static class ConfigModule
{
    public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        // Inject HttpClient
        services.AddHttpClient();
        
        // Inject Fluent Validation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(typeof(IModuleMarker));
        
        // Inject Swagger
        services.AddSwaggerGen();
    }
}