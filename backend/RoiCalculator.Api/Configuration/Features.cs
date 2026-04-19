namespace RoiCalculator.Api.Configuration;

public record FeatureFlags
{
    public bool UseInMemoryDatabase { get; init; }
}
