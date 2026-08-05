using Domain.Enums;

namespace Domain.Dtos;

public class IngredientReactionDto
{
    public Guid ProductId { get; set; } 
    public Guid IngredientId { get; set; } 
    public ReactionType Type { get; set; } 
    public int Severity { get; set; } 
    public string? Note { get; set; } 
}