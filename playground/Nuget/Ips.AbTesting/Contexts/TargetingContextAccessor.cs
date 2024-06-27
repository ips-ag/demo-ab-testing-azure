using Ips.AbTesting.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement.FeatureFilters;

namespace Ips.AbTesting.Contexts;

internal class TargetingContextAccessor(IHttpContextAccessor httpContextAccessor,
                                        ITargetingContextService distributionService) : ITargetingContextAccessor
{
    private const string TargetingContextLookup = $"{nameof(TargetingContextAccessor)}.{nameof(TargetingContext)}";

    public async ValueTask<TargetingContext> GetContextAsync()
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;
        if (httpContext.Items.TryGetValue(TargetingContextLookup, out var value))
        {
            return (TargetingContext)value!;
        }
        var targetingContext = await distributionService.GetTargetingContextAsync(httpContext.RequestAborted).ConfigureAwait(false);
        httpContext.Items[TargetingContextLookup] = targetingContext;
        return targetingContext;
    }
}
