
namespace Domain.Models;

public class ProductCategory 
{
    public required Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public required Guid CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;

}