using RoiForm.Domain.FormManagement.Entities;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiCalculator.Core.Tests.RoiForms;

public class FormFieldSchemaTests
{
    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = FormField.Create("revenue", "Revenue", FormFieldType.Number);

        result.IsSuccess.Should().BeTrue();
        result.Value.Key.Should().Be("revenue");
        result.Value.Label.Should().Be("Revenue");
    }

    [Theory]
    [InlineData("")]
    public void Create_WithEmptyIdentifier_Fails(string identifier)
    {
        var result = FormField.Create(identifier, "Label", FormFieldType.Text);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.field-identifier-cannot-be-empty");
    }

    [Fact]
    public void Create_WithIdentifierExceeding200Chars_Fails()
    {
        var result = FormField.Create(new string('x', 201), "Label", FormFieldType.Text);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.field-identifier-too-long");
    }

    [Theory]
    [InlineData("")]
    public void Create_WithEmptyLabel_Fails(string label)
    {
        var result = FormField.Create("identifier", label, FormFieldType.Text);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.field-label-cannot-be-empty");
    }

    [Fact]
    public void Create_WithLabelExceeding200Chars_Fails()
    {
        var result = FormField.Create("identifier", new string('x', 201), FormFieldType.Text);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.field-label-too-long");
    }
}
