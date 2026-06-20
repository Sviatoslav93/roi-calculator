using FunctionalPrimitives.Monads.Results;
using MediatR;
using RoiForm.Application.Features.RoiForms;
using Domain.Common;
using RoiForm.Domain.FormManagement;
using RoiForm.Domain.FormManagement.Entities;
using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiForm.Application.Features.RoiForms.Create;

public class CreateFormTemplateCommandHandler(IFormTemplateRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateFormTemplateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFormTemplateCommand request, CancellationToken cancellationToken)
    {
        if (await repository.ExistsWithNameAsync(request.Name, cancellationToken: cancellationToken))
        {
            return FormTemplateApplicationErrors.NameConflict(request.Name);
        }

        var formulaResult = Formula.Create(request.Formula);
        if (!formulaResult.IsSuccess)
            return formulaResult.Errors.ToArray();

        var formFields = new List<FormField>();
        foreach (var field in request.FormFields)
        {
            var fieldResult = FormField.Create(field.Key, field.Label, field.Type, field.Min, field.Max);
            if (!fieldResult.IsSuccess)
                return fieldResult.Errors.ToArray();

            formFields.Add(fieldResult.Value);
        }

        var formResult = FormTemplate.Create(request.Name, request.Title, formulaResult.Value, formFields);
        if (!formResult.IsSuccess)
            return formResult.Errors.ToArray();

        await repository.AddAsync(formResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return formResult.Value.Id;
    }
}
