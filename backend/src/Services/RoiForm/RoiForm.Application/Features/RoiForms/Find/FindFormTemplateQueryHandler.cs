using FunctionalPrimitives.Monads.Options;
using MediatR;
using RoiForm.Application.Features.RoiForms.Abstractions;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Application.Features.RoiForms.Find;

public class FindFormTemplateQueryHandler(IFormTemplateReadQueries queries)
    : IRequestHandler<FindFormTemplateQuery, Option<FormTemplateDto>>
{
    public Task<Option<FormTemplateDto>> Handle(FindFormTemplateQuery request, CancellationToken cancellationToken)
        => queries.FindByIdAsync(request.Id, cancellationToken);
}
