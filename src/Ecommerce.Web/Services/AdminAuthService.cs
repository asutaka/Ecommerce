using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Services;

public class AdminAuthService(EcommerceDbContext dbContext) : IAdminAuthService
{
    public async Task<AdminUser?> ValidateCredentialsAsync(string emailOrUsername, string password)
    {
        // Support login with both email and username
        var admin = await dbContext.AdminUsers
            .FirstOrDefaultAsync(x => (x.Email == emailOrUsername || x.Username == emailOrUsername) && x.IsActive);

        if (admin == null)
        {
            return null;
        }

        // Verify password using BCrypt
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
        
        return isValidPassword ? admin : null;
    }

    public async Task<AdminUser?> GetByIdAsync(Guid id)
    {
        return await dbContext.AdminUsers
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<AdminUser?> GetAdminByIdAsync(Guid id)
    {
        return await dbContext.AdminUsers
            .Include(x => x.Group)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateProfileAsync(Guid id, string email, string? fullName, string? currentPassword, string? newPassword)
    {
        var admin = await dbContext.AdminUsers.FindAsync(id);
        if (admin == null)
        {
            return false;
        }

        // If changing password, verify current password
        if (!string.IsNullOrEmpty(newPassword))
        {
            if (string.IsNullOrEmpty(currentPassword))
            {
                return false;
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(currentPassword, admin.PasswordHash);
            if (!isValidPassword)
            {
                return false;
            }

            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        }

        admin.Email = email;
        admin.FullName = fullName;
        admin.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return true;
    }
}
