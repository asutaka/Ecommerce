using Ecommerce.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public class EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<NotificationLog> Notifications => Set<NotificationLog>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();
    public DbSet<Coupon> Coupons => Set<Coupon>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(250);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Ignore(x => x.HeroImageUrl); // Computed property
            entity.Property(x => x.Images).HasColumnType("text[]"); // Postgres array

            entity.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(x => x.CustomerName).HasMaxLength(200);
            entity.Property(x => x.CustomerEmail).HasMaxLength(320);
            entity.Property(x => x.Total).HasPrecision(18, 2);
            entity.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
            
            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
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
            
            entity.HasIndex(x => x.SessionId);
            entity.HasIndex(x => x.CustomerId);
            
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
        
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200);
            entity.Property(x => x.Phone).HasMaxLength(20);
            
            entity.HasIndex(x => x.Email).IsUnique();
            
            entity.HasMany(x => x.Orders)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId);
            
            entity.HasMany(x => x.ShoppingCarts)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId);
            
            entity.HasMany(x => x.ExternalLogins)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId);
        });
        
        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.Property(x => x.Provider).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ProviderKey).HasMaxLength(256).IsRequired();
            
            entity.HasIndex(x => new { x.Provider, x.ProviderKey }).IsUnique();
        });
        
        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasOne(x => x.Customer)
                .WithMany(x => x.ShoppingCarts)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500).IsRequired();
            entity.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            entity.Property(x => x.DiscountPercentage).HasPrecision(5, 2);
            entity.Property(x => x.MinimumOrderAmount).HasPrecision(18, 2);
            
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.IsActive);
        });

        modelBuilder.SeedInitialData();
        modelBuilder.SeedCouponData();
        modelBuilder.SeedAdminUser();
    }
}

