using FluentValidation;

namespace RoiForm.Application.Features.RoiForms.Update;

public class UpdateFormTemplateCommandValidator : AbstractValidator<UpdateFormTemplateCommand>
{
    public UpdateFormTemplateCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Formula).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.FormFields).NotEmpty();

        RuleForEach(x => x.FormFields).ChildRules(field =>
        {
            field.RuleFor(f => f.Key).NotEmpty().MaximumLength(200);
            field.RuleFor(f => f.Label).NotEmpty().MaximumLength(200);
        });
    }
}
