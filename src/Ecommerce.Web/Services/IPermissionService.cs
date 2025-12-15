namespace Ecommerce.Web.Services;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string permissionKey);
    Task<Dictionary<string, int>> GetUserPermissionsAsync();
}
