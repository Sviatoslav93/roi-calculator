using RoiForm.Domain.FormManagement.Entities;
using RoiForm.Domain.FormManagement.Enums;
using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiCalculator.Core.Tests.RoiForms;

public class FormTemplateTests
{
    private static Formula ValidFormula() => Formula.Create("a + b").Value;
    private static FormField ValidField() => FormField.Create("a", "Field A", FormFieldType.Number).Value;

    // ── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = FormTemplate.Create("My Form", "Title", ValidFormula(), [ValidField()]);

        result.IsSuccess.Should().BeTrue();
        result.Value.Key.Should().Be("My Form");
        result.Value.Title.Should().Be("Title");
        result.Value.TemplateStatus.Should().Be(FormTemplateStatus.Draft);
        result.Value.FormFields.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("")]
    public void Create_WithEmptyName_Fails(string name)
    {
        var result = FormTemplate.Create(name, "Title", ValidFormula(), [ValidField()]);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.name-cannot-be-empty");
    }

    [Fact]
    public void Create_WithNameExceeding200Chars_Fails()
    {
        var result = FormTemplate.Create(new string('x', 201), "Title", ValidFormula(), [ValidField()]);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.name-too-long");
    }

    [Theory]
    [InlineData("")]
    public void Create_WithEmptyTitle_Fails(string title)
    {
        var result = FormTemplate.Create("Key", title, ValidFormula(), [ValidField()]);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.title-cannot-be-empty");
    }

    [Fact]
    public void Create_WithNoFields_Fails()
    {
        var result = FormTemplate.Create("Key", "Title", ValidFormula(), []);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.fields-cannot-be-empty");
    }

    // ── Publish ─────────────────────────────────────────────────────────────

    [Fact]
    public void Publish_WhenDraft_SetsStatusToPublished()
    {
        var form = FormTemplate.Create("Key", "Title", ValidFormula(), [ValidField()]).Value;

        var result = form.Publish();

        result.IsSuccess.Should().BeTrue();
        form.TemplateStatus.Should().Be(FormTemplateStatus.Published);
        form.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_Fails()
    {
        var form = FormTemplate.Create("Key", "Title", ValidFormula(), [ValidField()]).Value;
        form.Publish();

        var result = form.Publish();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.cannot-publish");
    }

    [Fact]
    public void Publish_WhenArchived_Fails()
    {
        var form = FormTemplate.Create("Key", "Title", ValidFormula(), [ValidField()]).Value;
        form.Publish();
        form.Archive();

        var result = form.Publish();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.cannot-publish");
    }

    // ── Unpublish ────────────────────────────────────────────────────────────

    [Fact]
    public void Unpublish_WhenPublished_SetsStatusToDraft()
    {
        var form = FormTemplate.Create("Key", "Title", ValidFormula(), [ValidField()]).Value;
        form.Publish();

        var result = form.Unpublish();

        result.IsSuccess.Should().BeTrue();
        form.TemplateStatus.Should().Be(FormTemplateStatus.Draft);
    }

    [Fact]
    public void Unpublish_WhenDraft_Fails()
    {
        var form = FormTemplate.Create("Key", "Title", ValidFormula(), [ValidField()]).Value;

        var result = form.Unpublish();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.cannot-unpublish");
    }

    // ── Archive ──────────────────────────────────────────────────────────────

    [Fact]
    public void Archive_WhenPublished_SetsStatusToArchived()
    {
        var form = FormTemplate.Create("Key", "Title", ValidFormula(), [ValidField()]).Value;
        form.Publish();

        var result = form.Archive();

        result.IsSuccess.Should().BeTrue();
        form.TemplateStatus.Should().Be(FormTemplateStatus.Archived);
    }

    [Fact]
    public void Archive_WhenDraft_Fails()
    {
        var form = FormTemplate.Create("Key", "Title", ValidFormula(), [ValidField()]).Value;

        var result = form.Archive();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Code == "roi-form.cannot-archive");
    }

    // ── Rename ───────────────────────────────────────────────────────────────

    [Fact]
    public void Rename_UpdatesNameAndTimestamp()
    {
        var form = FormTemplate.Create("Old Key", "Title", ValidFormula(), [ValidField()]).Value;

        form.Rename("New Key");

        form.Key.Should().Be("New Key");
        form.UpdatedAt.Should().NotBeNull();
    }

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public void Update_ReplacesContentAndTimestamp()
    {
        var form = FormTemplate.Create("Key", "Old Title", ValidFormula(), [ValidField()]).Value;
        var newFormula = Formula.Create("x * y").Value;
        var newField = FormField.Create("x", "Field X", FormFieldType.Text).Value;

        form.Update("New Title", newFormula, [newField]);

        form.Title.Should().Be("New Title");
        form.FormFields.Should().HaveCount(1);
        form.FormFields[0].Key.Should().Be("x");
        form.UpdatedAt.Should().NotBeNull();
    }
}
