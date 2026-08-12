using FunctionalPrimitives.Monads.Options;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Application.Features.FormTemplates.Find;

public interface IFindFormTemplateReader
{
    Task<Option<FormTemplateResponse>> Find(Guid id, CancellationToken cancellationToken = default);
}
