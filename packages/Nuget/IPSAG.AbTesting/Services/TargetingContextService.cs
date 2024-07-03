using Microsoft.FeatureManagement.FeatureFilters;

namespace IPSAG.AbTesting.Services;

public interface ITargetingContextService
{
    Task<TargetingContext> GetTargetingContextAsync(CancellationToken ct = default);
}