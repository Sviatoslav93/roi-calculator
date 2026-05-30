using RoiForm.Domain.Common;
using RoiForm.Domain.FormManagement.Entities;

namespace RoiForm.Domain.FormManagement;

public interface IFormTemplateRepository : IRepository<FormTemplate, Guid>
{
    Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
