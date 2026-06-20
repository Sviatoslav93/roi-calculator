using RoiForm.Application.Features.RoiForms.Rename;

namespace RoiForm.Api.Dtos.Requests;

public class RenameFormTemplateRequest
{
    public required string Name { get; init; }
}

public static class RenameFormTemplateMapper
{
    public static RenameFormTemplateCommand ToRenameFormTemplateCommand(this RenameFormTemplateRequest request, Guid id)
        => new(id, request.Name);
}
