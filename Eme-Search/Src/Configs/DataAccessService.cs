using Eme_Search.Database;
using Eme_Search.Infrastructures.Repositories;
using Eme_Search.Infrastructures.Repositories.Impl;
using Microsoft.EntityFrameworkCore;
    
namespace Eme_Search.Configs;

public static class DataAccessService
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        services.AddInfrastructure();

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");

        services.AddDbContext<SSDatabaseContext>(options =>
            options.UseNpgsql(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
    }
}