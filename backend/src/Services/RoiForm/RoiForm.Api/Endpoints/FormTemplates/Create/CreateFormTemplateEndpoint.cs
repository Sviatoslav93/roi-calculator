using MediatR;
using RoiForm.Api.Extensions;
using RoiForm.Application.Features.FormTemplates.Create;

namespace RoiForm.Api.Endpoints.FormTemplates.Create;

public static class CreateFormTemplateEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/form-templates", Handle)
            .WithTags("FormTemplates");
    }

    private static Task<IResult> Handle(
        CreateFormTemplateRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var command = MapToCommand(request);

        return sender.Send(command, ct)
            .ToHttpResultAsync(id => Results.Created($"/form-templates/{id}", id));
    }

   private static CreateFormTemplateCommand MapToCommand(CreateFormTemplateRequest request)
   {
       return new CreateFormTemplateCommand(
           request.Name,
           request.Title,
           request.Formula,
           [
               .. request.FormFields.Select(f => new CreateFormField(
                   f.Key,
                   f.Label,
                   f.Type,
                   f.Min,
                   f.Max,
                   f.IsRequired,
                   f.Order)),
           ]);
   }
}

