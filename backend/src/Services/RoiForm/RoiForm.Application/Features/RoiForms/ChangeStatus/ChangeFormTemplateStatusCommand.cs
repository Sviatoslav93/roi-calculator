using FunctionalPrimitives.Monads.Results;
using MediatR;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Application.Features.RoiForms.ChangeStatus;

public record ChangeFormTemplateStatusCommand(
    Guid Id,
    RoiFormStatus TargetStatus) : IRequest<Result<Unit>>;
