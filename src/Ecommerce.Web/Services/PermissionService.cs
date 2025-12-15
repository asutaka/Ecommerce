using System.Security.Claims;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Services;

public class PermissionService(
    EcommerceDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : IPermissionService
{
    public async Task<bool> HasPermissionAsync(string permissionKey)
    {
        var permissions = await GetUserPermissionsAsync();
        
        // Normalize permission key: remove ".view" suffix
        var normalizedKey = permissionKey
            .Replace(".view", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
        
        // Special case mappings to match database format
        var keyMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "carts", "Cart" },           // "carts.view" -> "Cart"
            { "adminusers", "AdminUsers" }, // "adminusers.view" -> "AdminUsers"
            { "products", "Products" },
            { "categories", "Categories" },
            { "orders", "Orders" },
            { "reports", "Reports" },
            { "settings", "Settings" },
            { "warehouses", "Warehouses" },
            { "suppliers", "Suppliers" },
            { "coupons", "Coupons" },
            { "groups", "Groups" },
            { "customers", "Customers" },
            { "dashboard", "Dashboard" }
        };
        
        // Try to find mapped key, otherwise capitalize first letter
        if (!keyMappings.TryGetValue(normalizedKey, out var mappedKey))
        {
            mappedKey = !string.IsNullOrEmpty(normalizedKey)
                ? char.ToUpper(normalizedKey[0]) + normalizedKey.Substring(1)
                : normalizedKey;
        }
        
        return permissions.ContainsKey(mappedKey) && permissions[mappedKey] > 0;
    }

    public async Task<Dictionary<string, int>> GetUserPermissionsAsync()
    {
        // Get current admin user ID from claims
        var userId = httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return new Dictionary<string, int>();

        // Get admin user with group
        var admin = await dbContext.AdminUsers
            .Include(x => x.Group)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

        return admin?.Group?.Permissions ?? new Dictionary<string, int>();
    }
}
