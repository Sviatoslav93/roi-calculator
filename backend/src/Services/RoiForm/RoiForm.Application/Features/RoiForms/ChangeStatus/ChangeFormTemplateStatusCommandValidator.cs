using FluentValidation;

namespace RoiForm.Application.Features.RoiForms.ChangeStatus;

public class ChangeFormTemplateStatusCommandValidator : AbstractValidator<ChangeFormTemplateStatusCommand>
{
    public ChangeFormTemplateStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TargetStatus).IsInEnum();
    }
}
