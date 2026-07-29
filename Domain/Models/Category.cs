using Domain.Common;

namespace Domain.Models;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}