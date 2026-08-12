using FunctionalPrimitives.Monads.Options.Extensions;
using MediatR;
using RoiForm.Application.Features.FormTemplates.Find;

namespace RoiForm.Api.Endpoints.FormTemplates.Find;

public static class FindFormTemplateEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}", Handle)
            .WithTags("FormTemplates");
    }

    private static Task<IResult> Handle(
        Guid id,
        ISender sender,
        CancellationToken ct)
    {
        var query = new FindFormTemplateQuery(id);

        return sender.Send(query, ct)
                 .MatchAsync(Results.Ok, () => Results.NotFound());
    }
}
