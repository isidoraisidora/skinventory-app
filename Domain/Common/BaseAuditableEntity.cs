namespace Domain.Common;

public class BaseAuditableEntity<TU> : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    
    public DateTime LastModifiedAt { get; set; }
    public string? LastModifiedById { get; set; }
}