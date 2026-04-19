namespace Armageddon.Server.Data.Models.AuditAndSoftDeleteModels
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        string? CreatedBy { get; set; }
        DateTime UpdatedAt { get; set; }
        string? UpdatedBy { get; set; }
        DateTime LastUpdatedAt { get; set; }
        string? LastUpdatedBy { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }
    }
}
