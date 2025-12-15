namespace Ecommerce.Infrastructure.Entities;

public class AdminUser : BaseEntity
{
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Group assignment
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
}
