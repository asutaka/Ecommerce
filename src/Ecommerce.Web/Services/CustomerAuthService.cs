using Ecommerce.Infrastructure.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Services;

public class CustomerAuthService(EcommerceDbContext dbContext) : ICustomerAuthService
{
    public async Task<Customer?> RegisterAsync(string email, string password, string? fullName = null, string? phone = null)
    {
        // Check if email already exists
        if (await dbContext.Customers.AnyAsync(x => x.Email == email))
        {
            return null;
        }

        var customer = new Customer
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = fullName,
            Phone = phone,
            EmailConfirmed = false
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();

        return customer;
    }

    public async Task<Customer?> ValidateCredentialsAsync(string email, string password)
    {
        var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Email == email);

        if (customer == null)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, customer.PasswordHash))
        {
            return null;
        }

        return customer;
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await dbContext.Customers.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await dbContext.Customers.FindAsync(id);
    }

    public async Task<Customer?> FindOrCreateExternalLoginAsync(string provider, string providerKey, string email, string? fullName = null)
    {
        // Try to find existing external login
        var externalLogin = await dbContext.ExternalLogins
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Provider == provider && x.ProviderKey == providerKey);

        if (externalLogin != null)
        {
            return externalLogin.Customer;
        }

        // Try to find customer by email
        var customer = await GetByEmailAsync(email);

        if (customer == null)
        {
            // Create new customer
            customer = new Customer
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Random password for social login
                FullName = fullName,
                EmailConfirmed = true // Social logins are pre-verified
            };

            dbContext.Customers.Add(customer);
        }

        // Create external login record
        var newExternalLogin = new ExternalLogin
        {
            Provider = provider,
            ProviderKey = providerKey,
            CustomerId = customer.Id
        };

        dbContext.ExternalLogins.Add(newExternalLogin);
        await dbContext.SaveChangesAsync();

        return customer;
    }

    public async Task UpdateLastLoginAsync(Guid customerId)
    {
        var customer = await dbContext.Customers.FindAsync(customerId);
        if (customer != null)
        {
            customer.LastLoginAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }
    }
}
