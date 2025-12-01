using Ecommerce.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public class EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<NotificationLog> Notifications => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(250);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Property(x => x.HeroImageUrl).HasMaxLength(1024);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(x => x.CustomerName).HasMaxLength(200);
            entity.Property(x => x.CustomerEmail).HasMaxLength(320);
            entity.Property(x => x.Total).HasPrecision(18, 2);
            entity.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(x => x.ProductName).HasMaxLength(200);
            entity.Property(x => x.UnitPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Reference).HasMaxLength(64);
        });

        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.Property(x => x.Channel).HasMaxLength(50);
            entity.Property(x => x.Destination).HasMaxLength(320);
        });

        modelBuilder.SeedInitialData();
    }
}

