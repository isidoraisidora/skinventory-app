using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class WishlistItem : BaseAuditableEntity
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public required Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public WishlistStatus WishlistStatus { get; set; }
}