namespace Ecommerce.Infrastructure.Entities;

public class Supplier : BaseEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public List<string> PhoneNumbers { get; set; } = new();
}
