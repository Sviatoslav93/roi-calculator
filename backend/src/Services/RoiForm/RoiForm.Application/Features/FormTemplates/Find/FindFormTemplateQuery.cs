using FunctionalPrimitives.Monads.Options;
using MediatR;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Application.Features.FormTemplates.Find;

public record FindFormTemplateQuery(Guid Id) : IRequest<Option<FormTemplateResponse>>;
