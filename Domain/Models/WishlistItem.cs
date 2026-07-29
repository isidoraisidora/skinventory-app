using Domain.Common;

namespace Domain.Models;

public class WishlistItem : BaseEntity
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public required Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public DateTime AddedAt { get; set; }
}