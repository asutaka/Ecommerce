namespace Ecommerce.Web.Helpers;

/// <summary>
/// Permission flags for role-based access control
/// Using bitwise flags to allow combining multiple permissions
/// </summary>
[Flags]
public enum EPermission
{
    None = 0,
    View = 1,      // 0001
    Add = 2,       // 0010
    Edit = 4,      // 0100
    Delete = 8,    // 1000
    Full = View | Add | Edit | Delete  // 1111 = 15
}
