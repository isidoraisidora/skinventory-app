using Domain.Common;

namespace Domain.Models;

public class EtlSyncLog : BaseEntity
{
    public required string JobName { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int ProductsImported { get; set; }
    public int ProductsSkipped { get; set; }
    public int ProductsUpdated{ get; set; }

}