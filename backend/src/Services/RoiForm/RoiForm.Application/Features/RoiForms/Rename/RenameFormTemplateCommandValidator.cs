using FluentValidation;
using RoiForm.Domain.FormManagement;

namespace RoiForm.Application.Features.RoiForms.Rename;

public class RenameFormTemplateCommandValidator : AbstractValidator<RenameFormTemplateCommand>
{
    public RenameFormTemplateCommandValidator(IFormTemplateRepository repository)
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync((cmd, name, ct) => repository.ExistsWithNameAsync(name, cmd.Id, ct).ContinueWith(t => !t.Result, ct))
            .WithMessage("A form template with this name already exists.");
    }
}
