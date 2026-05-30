using FunctionalPrimitives.Monads.Results;
using MediatR;
using RoiForm.Domain.Common;
using RoiForm.Domain.FormManagement;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Application.Features.RoiForms.ChangeStatus;

public class ChangeFormTemplateStatusCommandHandler(IFormTemplateRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeFormTemplateStatusCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ChangeFormTemplateStatusCommand command, CancellationToken cancellationToken)
    {
        var form = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (form is null)
            return FormTemplateApplicationErrors.FormNotFound(command.Id);

        var result = command.TargetStatus switch
        {
            RoiFormStatus.Published => form.Publish(),
            RoiFormStatus.Draft => form.Unpublish(),
            RoiFormStatus.Archived => form.Archive(),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command.TargetStatus, null)
        };

        if (!result.IsSuccess)
            return result.Errors.ToArray();

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
