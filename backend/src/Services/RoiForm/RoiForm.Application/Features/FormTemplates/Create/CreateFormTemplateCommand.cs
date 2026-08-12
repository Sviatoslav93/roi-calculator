using FunctionalPrimitives.Monads.Results;
using MediatR;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Application.Features.FormTemplates.Create;

public record CreateFormTemplateCommand(
    string Name,
    string Title,
    string Formula,
    IReadOnlyList<CreateFormField> FormFields) : IRequest<Result<Guid>>;

public record CreateFormField(
    string Key,
    string Label,
    FormFieldType Type,
    decimal? Min,
    decimal? Max,
    bool IsRequired,
    int Order);
