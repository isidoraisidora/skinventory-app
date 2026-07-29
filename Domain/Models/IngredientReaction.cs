using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class IngredientReaction : BaseEntity
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public required Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public required Guid IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;

    public ReactionType ReactionType { get; set; }
    public string? Note { get; set; }
    
    [Range(1,10)]
    public int ReactionSeverity { get; set; }
    
}