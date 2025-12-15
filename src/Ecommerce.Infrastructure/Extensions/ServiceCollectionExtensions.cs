using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Postgres connection string is not configured. Please add it to appsettings.json under 'ConnectionStrings:Postgres'.");

        // Configure Npgsql to support dynamic JSON types (for Dictionary<string, int> in JSONB)
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<EcommerceDbContext>(options =>
        {
            options.UseNpgsql(dataSource, builder =>
            {
                builder.MigrationsHistoryTable("__ef_migrations_history", "ecommerce");
            });
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }
}

