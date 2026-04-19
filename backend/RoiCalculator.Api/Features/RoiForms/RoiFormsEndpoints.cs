using MediatR;
using RoiCalculator.Api.Common.Query;
using RoiCalculator.Core.Aggregates;

namespace RoiCalculator.Api.Features.RoiForms;

public record UpdateNameRequest(string Name);
public record ChangeStatusRequest(RoiFormStatus Status);

public static class RoiFormsEndpoints
{
    public static void MapRoiFormEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/roi-forms").WithTags("RoiForms");

        group.MapPost("/", Create);
        group.MapGet("/{id:guid}", GetById);
        group.MapGet("/", List);
        group.MapPatch("/{id:guid}/name", UpdateName);
        group.MapPatch("/{id:guid}/status", ChangeStatus);
    }

    private static async Task<IResult> Create(
        CreateRoiFormRequest command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/roi-forms/{result.Id}", result);
    }

    private static async Task<IResult> GetById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRoiFormByIdQuery(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> List(
        ISender sender,
        CancellationToken cancellationToken,
        string? name = null,
        string? sortBy = null,
        SortDirection sortDirection = SortDirection.Descending,
        int page = 1,
        int pageSize = 20)
    {
        var result = await sender.Send(new ListRoiFormsQuery(name, sortBy, sortDirection, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateName(
        Guid id,
        UpdateNameRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateRoiFormNameCommand(id, request.Name), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> ChangeStatus(
        Guid id,
        ChangeStatusRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeRoiFormStatusCommand(id, request.Status), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}
