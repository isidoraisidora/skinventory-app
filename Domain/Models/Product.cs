using Domain.Common;

namespace Domain.Models;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Brand { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public string? Barcode { get; set; }

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();


}