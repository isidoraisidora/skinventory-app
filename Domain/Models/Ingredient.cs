using Domain.Common;

namespace Domain.Models;

public class Ingredient : BaseEntity
{
    public required string Name { get; set; }
    public required string InciName { get; set; }
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}