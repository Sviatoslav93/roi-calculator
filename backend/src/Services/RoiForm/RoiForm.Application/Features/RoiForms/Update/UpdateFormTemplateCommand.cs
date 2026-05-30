using FunctionalPrimitives.Monads.Results;
using MediatR;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Application.Features.RoiForms.Update;

public record UpdateFormFieldInput(
    string Key,
    string Label,
    FormFieldType Type,
    decimal? Min,
    decimal? Max);

public record UpdateFormTemplateCommand(
    Guid Id,
    string Title,
    string Formula,
    IReadOnlyList<UpdateFormFieldInput> FormFields) : IRequest<Result<Unit>>;
