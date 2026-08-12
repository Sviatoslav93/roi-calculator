using System.Text.Json;

namespace RoiForm.Api.Tests.RoiForms;

public class RoiFormsTests(RoiCalculatorFactory factory) : IClassFixture<RoiCalculatorFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // ── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_WithValidRequest_Returns201WithId()
    {
        var response = await _client.PostAsJsonAsync("/form-templates", RoiFormFixture.ValidCreateRequest());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_WithDuplicateName_Returns409()
    {
        var name = $"Unique {Guid.NewGuid():N}";
        await _client.PostAsJsonAsync("/form-templates", RoiFormFixture.ValidCreateRequest(name));

        var response = await _client.PostAsJsonAsync("/form-templates", RoiFormFixture.ValidCreateRequest(name));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_WithInvalidFormula_Returns4xx()
    {
        var request = new
        {
            name = $"Form {Guid.NewGuid():N}",
            title = "Title",
            formula = "",
            formFields = new[] { new { key = "a", label = "A", type = 2 } }
        };

        var response = await _client.PostAsJsonAsync("/form-templates", request);

        ((int)response.StatusCode).Should().BeGreaterThanOrEqualTo(400);
    }

    // ── GetById ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_WhenExists_Returns200WithDto()
    {
        var id = await CreateFormAndGetId();

        var response = await _client.GetAsync($"/form-templates/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("id");
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync($"/form-templates/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── List ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task List_Returns200WithPagedResult()
    {
        var response = await _client.GetAsync("/form-templates");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── Rename ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Rename_WithNewName_Returns200()
    {
        var id = await CreateFormAndGetId();

        var response = await _client.PatchAsJsonAsync($"/form-templates/{id}/name",
            new { name = $"Renamed {Guid.NewGuid():N}" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Rename_ToDuplicateName_Returns4xx()
    {
        var existingName = $"Existing {Guid.NewGuid():N}";
        await _client.PostAsJsonAsync("/form-templates", RoiFormFixture.ValidCreateRequest(existingName));
        var id = await CreateFormAndGetId();

        var response = await _client.PatchAsJsonAsync($"/form-templates/{id}/name", new { name = existingName });

        ((int)response.StatusCode).Should().BeGreaterThanOrEqualTo(400);
    }

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_WithValidRequest_Returns200()
    {
        var id = await CreateFormAndGetId();

        var response = await _client.PatchAsJsonAsync($"/form-templates/{id}", RoiFormFixture.ValidUpdateRequest());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── ChangeStatus ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ChangeStatus_DraftToPublished_Returns200()
    {
        var id = await CreateFormAndGetId();

        var response = await _client.PatchAsJsonAsync($"/form-templates/{id}/templateStatus", new { status = "Published" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangeStatus_InvalidTransition_Returns4xx()
    {
        var id = await CreateFormAndGetId();
        await _client.PatchAsJsonAsync($"/form-templates/{id}/templateStatus", new { status = "Published" });

        var response = await _client.PatchAsJsonAsync($"/form-templates/{id}/templateStatus", new { status = "Published" });

        ((int)response.StatusCode).Should().BeGreaterThanOrEqualTo(400);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<Guid> CreateFormAndGetId()
    {
        var response = await _client.PostAsJsonAsync("/form-templates", RoiFormFixture.ValidCreateRequest());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}
