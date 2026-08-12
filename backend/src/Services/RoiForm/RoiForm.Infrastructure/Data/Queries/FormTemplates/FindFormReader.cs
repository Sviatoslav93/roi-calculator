using Dapper;
using FunctionalPrimitives.Monads.Options;
using RoiForm.Application.Features.FormTemplates.Find;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Infrastructure.Data.Queries.FormTemplates;

public class FindFormReader(IDbConnectionFactory dbFactory) : IFindFormTemplateReader
{
    public async Task<Option<FormTemplateResponse>> Find(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await dbFactory.OpenConnectionAsync(cancellationToken);

        var form = await connection.QueryFirstOrDefaultAsync<FormTemplateResponse>(
            """
            SELECT Id, Key, Title, TemplateStatus, FormulaExpression, CreatedAt, UpdatedAt
            FROM roi_form_templates
            WHERE Id = @Id
            """,
            new { Id = id });

        if (form is null)
            return None<FormTemplateResponse>();

        var fields = await connection.QueryAsync<FormFieldResponse>(
            "SELECT Key, Label, Type, Min, Max FROM roi_form_fields WHERE RoiFormId = @RoiFormId",
            new { RoiFormId = id });

        return form with
        {
            FormFields = [..fields],
        };
    }
}
