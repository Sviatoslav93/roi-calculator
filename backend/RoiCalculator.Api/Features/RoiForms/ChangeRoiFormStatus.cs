using FluentValidation;
using MediatR;
using RoiCalculator.Core.Aggregates;
using RoiCalculator.Core.Common;

namespace RoiCalculator.Api.Features.RoiForms;

public record ChangeRoiFormStatusCommand(Guid Id, RoiFormStatus TargetStatus) : IRequest<RoiFormDto?>;

public class ChangeRoiFormStatusValidator : AbstractValidator<ChangeRoiFormStatusCommand>
{
    public ChangeRoiFormStatusValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TargetStatus).IsInEnum();
    }
}

public class ChangeRoiFormStatusHandler(IRoiFormRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeRoiFormStatusCommand, RoiFormDto?>
{
    public async Task<RoiFormDto?> Handle(ChangeRoiFormStatusCommand request, CancellationToken cancellationToken)
    {
        var form = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (form is null) return null;

        switch (request.TargetStatus)
        {
            case RoiFormStatus.Published:
                form.Publish();
                break;
            case RoiFormStatus.Draft:
                form.Unpublish();
                break;
            case RoiFormStatus.Archived:
                form.Archive();
                break;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return form.ToDto();
    }
}
