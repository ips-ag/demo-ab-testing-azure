using Microsoft.FeatureManagement.FeatureFilters;

namespace IPS.AbTesting.Services;

public interface ITargetingContextService
{
    Task<TargetingContext> GetTargetingContextAsync(CancellationToken ct = default);
}