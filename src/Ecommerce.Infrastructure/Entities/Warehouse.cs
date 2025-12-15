namespace Ecommerce.Infrastructure.Entities;

public class Warehouse : BaseEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public List<string> PhoneNumbers { get; set; } = new();
}
