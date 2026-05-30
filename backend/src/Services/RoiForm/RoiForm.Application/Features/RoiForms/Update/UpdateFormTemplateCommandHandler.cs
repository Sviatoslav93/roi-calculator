using FunctionalPrimitives.Monads.Results;
using MediatR;
using RoiForm.Domain.Common;
using RoiForm.Domain.FormManagement;
using RoiForm.Domain.FormManagement.Entities;
using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiForm.Application.Features.RoiForms.Update;

public class UpdateFormTemplateCommandHandler(IFormTemplateRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateFormTemplateCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateFormTemplateCommand command, CancellationToken cancellationToken)
    {
        var form = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (form is null)
            return FormTemplateApplicationErrors.FormNotFound(command.Id);

        var formulaResult = Formula.Create(command.Formula);
        if (!formulaResult.IsSuccess)
            return formulaResult.Errors.ToArray();

        var formFields = new List<FormField>();
        foreach (var field in command.FormFields)
        {
            var fieldResult = FormField.Create(field.Key, field.Label, field.Type, field.Min, field.Max);
            if (!fieldResult.IsSuccess)
                return fieldResult.Errors.ToArray();
            formFields.Add(fieldResult.Value);
        }

        form.Update(command.Title, formulaResult.Value, formFields);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
