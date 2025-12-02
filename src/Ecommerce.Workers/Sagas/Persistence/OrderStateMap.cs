using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Workers.Sagas.Persistence;

public class OrderStateMap : SagaClassMap<OrderState>
{
    protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
    {
        entity.ToTable("order_sagas");
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.InvoiceNumber).HasMaxLength(64);
        entity.Property(x => x.PaymentReference).HasMaxLength(64);
    }
}

public class OrderStateDbContext : SagaDbContext
{
    public OrderStateDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }
}

