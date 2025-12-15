namespace Ecommerce.Infrastructure.Entities;

public class Group : BaseEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Module permissions stored as Dictionary<ModuleName, PermissionFlags>
    /// Example: {"Products": 15, "Orders": 7} where 15=Full, 7=View+Add+Edit
    /// Stored as JSONB in PostgreSQL
    /// </summary>
    public Dictionary<string, int> Permissions { get; set; } = new();
}
