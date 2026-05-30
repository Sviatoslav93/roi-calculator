using FunctionalPrimitives.Monads.Results;
using MediatR;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Application.Features.RoiForms.Create;

public record FormFieldInput(
    string Key,
    string Label,
    FormFieldType Type,
    decimal? Min,
    decimal? Max,
    bool IsRequired,
    string? Placeholder,
    string? HelpText,
    int Order);

public record CreateFormTemplateCommand(
    string Name,
    string Title,
    string Formula,
    IReadOnlyList<FormFieldInput> FormFields) : IRequest<Result<Guid>>;
