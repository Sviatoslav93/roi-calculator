using FunctionalPrimitives.Monads.Options.Extensions;
using MediatR;
using RoiForm.Api.Dtos.Requests;
using RoiForm.Api.Extensions;
using RoiForm.Application.Features.RoiForms.ChangeStatus;
using RoiForm.Application.Features.RoiForms.Find;
using RoiForm.Application.Features.RoiForms.List;

namespace RoiForm.Api.Endpoints;

public static class FormTemplatesEndpoints
{
    public static void MapFormTemplateEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/form-templates").WithTags("FormTemplates");

        group.MapPost("/", Create);
        group.MapGet("/{id:guid}", GetById);
        group.MapGet("/", List);
        group.MapPatch("/{id:guid}", Update);
        group.MapPatch("/{id:guid}/name", Rename);
        group.MapPatch("/{id:guid}/status", ChangeStatus);
    }

    private static Task<IResult> Create(
        CreateFormTemplateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(), cancellationToken)
            .ToHttpResultAsync(id => Results.Created($"/form-templates/{id}", id));
    }

    private static Task<IResult> GetById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new FindFormTemplateQuery(id), cancellationToken)
            .MatchAsync(
                Results.Ok,
                () => Results.NotFound());
    }

    private static async Task<IResult> List(
        ISender sender,
        [AsParameters] ListFormTemplatesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(request.ToQuery(), cancellationToken);
        return Results.Ok(result);
    }

    private static Task<IResult> Update(
        Guid id,
        UpdateFormTemplateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(id), cancellationToken)
            .ToHttpResultAsync();
    }

    private static Task<IResult> Rename(
        Guid id,
        RenameFormTemplateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(id), cancellationToken)
            .ToHttpResultAsync();
    }

    private static async Task<IResult> ChangeStatus(
        Guid id,
        ChangeStatusRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new ChangeFormTemplateStatusCommand(id, request.Status), cancellationToken)
            .ToHttpResultAsync();
    }
}
