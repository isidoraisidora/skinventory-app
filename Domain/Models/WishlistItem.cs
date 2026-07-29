using Domain.Common;

namespace Domain.Models;

public class WishlistItem : BaseAuditableEntity<User>
{
    public required Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}