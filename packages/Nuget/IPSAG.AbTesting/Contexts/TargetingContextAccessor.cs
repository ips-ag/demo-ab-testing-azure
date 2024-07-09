using IPSAG.AbTesting.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement.FeatureFilters;

namespace IPSAG.AbTesting.Contexts;

internal class TargetingContextAccessor(IHttpContextAccessor httpContextAccessor,
                                        ITargetingContextService distributionService) : ITargetingContextAccessor
{
    private const string TargetingContextLookup = $"{nameof(TargetingContextAccessor)}.{nameof(TargetingContext)}";
    private static string DefaultHeader = $"{nameof(AbTesting)}-{nameof(TargetingContext.Groups)}".ToLower();

    public async ValueTask<TargetingContext> GetContextAsync()
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;
        if (httpContext.Items.TryGetValue(TargetingContextLookup, out var value))
        {
            return (TargetingContext)value!;
        }
        var targetingContext = await distributionService.GetTargetingContextAsync(httpContext.RequestAborted).ConfigureAwait(false);
        httpContext.Response.Headers.Append(DefaultHeader, targetingContext.Groups.Any() ? string.Join(";", targetingContext.Groups) : "Anonymous");
        httpContext.Items[TargetingContextLookup] = targetingContext;
        return targetingContext;
    }
}
