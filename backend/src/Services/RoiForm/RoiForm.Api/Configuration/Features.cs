namespace RoiForm.Api.Configuration;

public record FeatureFlags
{
    public bool UseInMemoryDatabase { get; init; }
}
