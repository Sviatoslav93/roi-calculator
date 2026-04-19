using FluentValidation;
using MediatR;
using RoiCalculator.Core.Aggregates;
using RoiCalculator.Core.Common;

namespace RoiCalculator.Api.Features.RoiForms;

public record CreateRoiFormRequest(string Name) : IRequest<RoiFormDto>;

public class CreateRoiFormValidator : AbstractValidator<CreateRoiFormRequest>
{
    public CreateRoiFormValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class CreateRoiFormHandler(IRoiFormRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoiFormRequest, RoiFormDto>
{
    public async Task<RoiFormDto> Handle(CreateRoiFormRequest request, CancellationToken cancellationToken)
    {
        var form = RoiForm.Create(request.Name);
        await repository.AddAsync(form, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return form.ToDto();
    }
}
