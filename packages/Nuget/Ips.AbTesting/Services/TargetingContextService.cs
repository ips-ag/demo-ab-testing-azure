using Microsoft.FeatureManagement.FeatureFilters;

namespace Ips.AbTesting.Services;

public interface ITargetingContextService
{
    Task<TargetingContext> GetTargetingContextAsync(CancellationToken ct = default);
}