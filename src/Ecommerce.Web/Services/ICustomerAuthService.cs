using Ecommerce.Infrastructure.Entities;

namespace Ecommerce.Web.Services;

public interface ICustomerAuthService
{
    Task<Customer?> RegisterAsync(string email, string password, string? fullName = null, string? phone = null);
    Task<Customer?> ValidateCredentialsAsync(string email, string password);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> FindOrCreateExternalLoginAsync(string provider, string providerKey, string email, string? fullName = null);
    Task UpdateLastLoginAsync(Guid customerId);
}
