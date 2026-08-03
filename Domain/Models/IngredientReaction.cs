using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class IngredientReaction : BaseAuditableEntity
{ 
    public required Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public required Guid IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;

    public ReactionType ReactionType { get; set; }
    public string? Note { get; set; }
    
    public int ReactionSeverity { get; set; }
}