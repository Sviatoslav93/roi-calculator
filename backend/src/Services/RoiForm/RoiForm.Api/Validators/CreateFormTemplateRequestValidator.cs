using FluentValidation;
using RoiForm.Api.Dtos.Requests;

namespace RoiForm.Api.Validators;

public class CreateFormTemplateRequestValidator : AbstractValidator<CreateFormTemplateRequest>
{
    public CreateFormTemplateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Formula).NotEmpty().MaximumLength(4000);
        RuleForEach(x => x.FormFields).ChildRules(field =>
        {
            field.RuleFor(x => x.Label).NotEmpty().MaximumLength(200);
            field.RuleFor(x => x.Key).NotEmpty().MaximumLength(100);
            field.RuleFor(x => x.Type).IsInEnum();
        });
    }
}
