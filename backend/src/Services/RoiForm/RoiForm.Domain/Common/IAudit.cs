namespace RoiForm.Domain.Common;

public interface IAudit
{
    string CreatedBy { get; }
    DateTime CreatedAt { get; }

    string? UpdatedBy { get; }
    DateTime? UpdatedAt { get; }

    void SetCreatedInfo(string createdBy, DateTime createdAt);
    void SetUpdatedInfo(string? updatedBy, DateTime? updatedAt);
}
