using FunctionalPrimitives.Monads.Results;
using MediatR;
using Domain.Common;
using RoiForm.Domain.FormManagement;

namespace RoiForm.Application.Features.RoiForms.Rename;

public class RenameFormTemplateCommandHandler(IFormTemplateRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<RenameFormTemplateCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(RenameFormTemplateCommand command, CancellationToken cancellationToken)
    {
        var form = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (form is null)
            return FormTemplateApplicationErrors.FormNotFound(command.Id);

        if (await repository.ExistsWithNameAsync(command.Name, command.Id, cancellationToken))
            return FormTemplateApplicationErrors.NameConflict(command.Name);

        form.Rename(command.Name);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
