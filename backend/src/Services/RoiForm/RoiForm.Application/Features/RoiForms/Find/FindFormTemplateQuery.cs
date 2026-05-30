using FunctionalPrimitives.Monads.Options;
using MediatR;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Application.Features.RoiForms.Find;

public record FindFormTemplateQuery(Guid Id) : IRequest<Option<FormTemplateDto>>;
