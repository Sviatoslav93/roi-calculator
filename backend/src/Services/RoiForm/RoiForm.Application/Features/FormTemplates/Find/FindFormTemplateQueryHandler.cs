using FunctionalPrimitives.Monads.Options;
using MediatR;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Application.Features.FormTemplates.Find;

public class FindFormTemplateQueryHandler(IFindFormTemplateReader reader)
    : IRequestHandler<FindFormTemplateQuery, Option<FormTemplateResponse>>
{
    public Task<Option<FormTemplateResponse>> Handle(
        FindFormTemplateQuery query,
        CancellationToken cancellationToken) => reader.Find(query.Id, cancellationToken);
}
