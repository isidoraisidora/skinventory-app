using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class InventoryItem : BaseEntity
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public required Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public DateTime AddedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Comment { get; set; }
    public int? Rating { get; set; }
    public ProductStatus ProductStatus { get; set; }
    
    public DateTime? OpenedDate { get; set; }
    public int? PaoMonths { get; set; }
}