using Ecommerce.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public static class SeedData
{
    public static void SeedAdminUser(this ModelBuilder modelBuilder)
    {
        // Create default admin user
        // Password: Admin@123
        var adminUser = new AdminUser
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Username = "admin",
            Email = "admin@ecommerce.com",
            FullName = "Administrator",
            // BCrypt hash of "Admin@123"
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        modelBuilder.Entity<AdminUser>().HasData(adminUser);
    }
}
