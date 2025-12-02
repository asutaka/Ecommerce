using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Services;

public class AdminAuthService(EcommerceDbContext dbContext) : IAdminAuthService
{
    public async Task<AdminUser?> ValidateCredentialsAsync(string username, string password)
    {
        var admin = await dbContext.AdminUsers
            .FirstOrDefaultAsync(x => x.Username == username && x.IsActive);

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
}
