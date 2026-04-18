using MediatR;
using Microsoft.EntityFrameworkCore;
using RoiCalculator.Infrastructure.Data;

namespace RoiCalculator.Api.Features.RoiForms;

public record GetRoiFormByIdQuery(Guid Id) : IRequest<RoiFormDto?>;

public class GetRoiFormByIdHandler(AppDbContext context)
    : IRequestHandler<GetRoiFormByIdQuery, RoiFormDto?>
{
    public async Task<RoiFormDto?> Handle(GetRoiFormByIdQuery request, CancellationToken cancellationToken)
    {
        var form = await context.RoiForms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return form?.ToDto();
    }
}
