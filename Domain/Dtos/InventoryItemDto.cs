namespace Domain.Dtos;

public class InventoryItemDto
{
    public Guid ProductId { get; set; }
    public string? Comment { get; set; }
    public int? Rating { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? OpenedDate { get; set; }
    public int? PaoMonths { get; set; }
}