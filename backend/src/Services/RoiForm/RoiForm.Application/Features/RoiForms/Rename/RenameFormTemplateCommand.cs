using FunctionalPrimitives.Monads.Results;
using MediatR;

namespace RoiForm.Application.Features.RoiForms.Rename;

public record RenameFormTemplateCommand(Guid Id, string Name) : IRequest<Result<Unit>>;
