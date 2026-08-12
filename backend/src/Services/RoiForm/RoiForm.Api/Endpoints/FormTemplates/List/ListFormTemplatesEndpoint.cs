using MediatR;
using RoiForm.Api.Dtos.Requests;
using RoiForm.Application.Common.Query;
using RoiForm.Application.Features.FormTemplates.List;

namespace RoiForm.Api.Endpoints.List;

public class ListFormTemplatesEndpoint
{

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", Handle)
            .WithTags("FormTemplates");
    }

    private static async Task<IResult> Handle(
        [AsParameters] ListFormTemplatesRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var query = MapToQuery(request);

        var response = await sender.Send(query, ct);

        return Results.Ok(response);
    }

    private static ListFormTemplatesQuery MapToQuery(ListFormTemplatesRequest request)
    {
        return new ListFormTemplatesQuery(
            request.Name,
            request.SortBy,
            request.SortDirection ?? SortDirection.Descending,
            request.Page,
            request.PageSize);
    }
}
