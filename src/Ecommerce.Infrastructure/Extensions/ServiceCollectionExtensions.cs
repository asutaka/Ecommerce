using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
                               ?? configuration["Database:ConnectionString"]
                               ?? "Host=localhost;Database=ecommerce;Username=postgres;Password=postgres";

        services.AddDbContext<EcommerceDbContext>(options =>
        {
            options.UseNpgsql(connectionString, builder =>
            {
                builder.MigrationsHistoryTable("__ef_migrations_history", "ecommerce");
            });
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }
}

