using Ecommerce.Infrastructure.Extensions;
using Ecommerce.Workers.Sagas;
using Ecommerce.Workers.Sagas.Persistence;
using Ecommerce.Workers.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddSagaStateMachine<OrderSaga, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            r.AddDbContext<DbContext, OrderStateDbContext>((provider, optionsBuilder) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("Postgres")
                                       ?? configuration["Database:ConnectionString"]
                                       ?? "Host=localhost;Database=ecommerce;Username=postgres;Password=postgres";

                optionsBuilder.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(OrderStateDbContext).Assembly.FullName);
                });
            });
        });

    x.AddConsumer<CreateInvoiceConsumer>();
    x.AddConsumer<ProcessPaymentConsumer>();
    x.AddConsumer<SendNotificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitSection = context.GetRequiredService<IConfiguration>().GetSection("RabbitMq");
        var host = rabbitSection.GetValue<string>("Host") ?? "localhost";
        var username = rabbitSection.GetValue<string>("Username") ?? "guest";
        var password = rabbitSection.GetValue<string>("Password") ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
await host.RunAsync();
