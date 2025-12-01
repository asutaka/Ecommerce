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
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(250);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Ignore(x => x.HeroImageUrl); // Computed property
            entity.Property(x => x.Images).HasColumnType("text[]"); // Postgres array
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

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.Property(x => x.SessionId).HasMaxLength(128).IsRequired();
            entity.Property(x => x.UserId).IsRequired(false);
            
            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => x.UserId);
            
            entity.HasMany(x => x.Items)
                .WithOne(x => x.Cart)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.Property(x => x.ProductName).HasMaxLength(250).IsRequired();
            entity.Property(x => x.ProductImageUrl).HasMaxLength(1024).IsRequired();
            entity.Property(x => x.UnitPrice).HasPrecision(18, 2);
            
            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.Ignore(x => x.LineTotal);
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.Property(x => x.Username).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200);
            
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.SeedInitialData();
        modelBuilder.SeedAdminUser();
    }
}

