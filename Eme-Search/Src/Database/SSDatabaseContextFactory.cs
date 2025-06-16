using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MongoDB.Driver;

namespace Eme_Search.Database;

internal class SSDatabaseContextFactory : IDesignTimeDbContextFactory<SSDatabaseContext>
{
    public SSDatabaseContext CreateDbContext(string[] args)
    {
        var config = BuildConfiguration();
        var optionsBuilder = new DbContextOptionsBuilder<SSDatabaseContext>();
        var connectionString = config.GetConnectionString("Postgres");
            
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(180);
        });

        return new SSDatabaseContext(optionsBuilder.Options);
    }
    
    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}