using System.Text.Json;
using Ecommerce.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ecommerce.Infrastructure.Persistence;

public class EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
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
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Group> Groups => Set<Group>();

    // In OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(250);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Ignore(x => x.HeroImageUrl);
            entity.Property(x => x.Images).HasColumnType("text[]");

            entity.HasOne(x => x.PrimaryCategory)
                .WithMany()
                .HasForeignKey(x => x.PrimaryCategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.Property(x => x.SKU).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Color).HasMaxLength(50);
            entity.Property(x => x.Size).HasMaxLength(20);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Property(x => x.ImageUrl).HasMaxLength(500);

            // Unique index on SKU
            entity.HasIndex(x => x.SKU).IsUnique();
            
            // Index on ProductId for faster queries
            entity.HasIndex(x => x.ProductId);

            // Relationship with Product
            entity.HasOne(x => x.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });

            entity.HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            entity.HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
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
            
            // Foreign key to Group
            entity.HasOne(x => x.Group)
                .WithMany()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
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

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(500).IsRequired();
            entity.Property(x => x.PhoneNumbers).HasColumnType("text[]");

            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(500).IsRequired();
            entity.Property(x => x.PhoneNumbers).HasColumnType("text[]");

            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Permissions)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, int>(),
                      new ValueComparer<Dictionary<string, int>>(
                          (c1, c2) => c1!.SequenceEqual(c2!),
                          c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                          c => c.ToDictionary(e => e.Key, e => e.Value)
                      )
                  )
                  .HasDefaultValueSql("'{}'::jsonb");  // Default to empty JSON object

            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.IsActive);
        });

        modelBuilder.SeedInitialData();
        modelBuilder.SeedCouponData();
        modelBuilder.SeedAdminUser();
    }
}

