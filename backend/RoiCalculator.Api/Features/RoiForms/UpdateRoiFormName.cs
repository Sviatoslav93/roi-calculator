using FluentValidation;
using MediatR;
using RoiCalculator.Core.Aggregates;
using RoiCalculator.Core.Common;

namespace RoiCalculator.Api.Features.RoiForms;

public record UpdateRoiFormNameCommand(Guid Id, string Name) : IRequest<RoiFormDto?>;

public class UpdateRoiFormNameValidator : AbstractValidator<UpdateRoiFormNameCommand>
{
    public UpdateRoiFormNameValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class UpdateRoiFormNameHandler(IRoiFormRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRoiFormNameCommand, RoiFormDto?>
{
    public async Task<RoiFormDto?> Handle(UpdateRoiFormNameCommand request, CancellationToken cancellationToken)
    {
        var form = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (form is null) return null;

        form.UpdateName(request.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return form.ToDto();
    }
}
