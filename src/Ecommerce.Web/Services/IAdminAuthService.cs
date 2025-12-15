using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Web.Services;

public interface IAdminAuthService
{
    Task<AdminUser?> ValidateCredentialsAsync(string username, string password);
    Task<AdminUser?> GetByIdAsync(Guid id);
    Task<AdminUser?> GetAdminByIdAsync(Guid id);
    Task<bool> UpdateProfileAsync(Guid id, string email, string? fullName, string? currentPassword, string? newPassword);
}
