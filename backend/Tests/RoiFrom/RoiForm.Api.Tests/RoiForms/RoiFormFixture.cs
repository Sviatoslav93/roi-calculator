namespace RoiForm.Api.Tests.RoiForms;

public static class RoiFormFixture
{
    public static object ValidCreateRequest(string? name = null) => new
    {
        name = name ?? $"Form {Guid.NewGuid():N}",
        title = "Test Title",
        formula = "a + b",
        formFields = new[] { new { key = "a", label = "Field A", type = 2 } }
    };

    public static object ValidUpdateRequest() => new
    {
        title = "Updated Title",
        formula = "x * y",
        formFields = new[] { new { key = "x", label = "Field X", type = 2 } }
    };
}
