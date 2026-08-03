namespace Domain.Common;

public class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    
    public DateTime LastModifiedAt { get; set; }
    public Guid? LastModifiedById { get; set; }
}