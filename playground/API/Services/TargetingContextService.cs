using Ips.AbTesting.Services;
using Microsoft.FeatureManagement.FeatureFilters;
using System.Security.Claims;

namespace API.Services;

public class TargetingContextService(IHttpContextAccessor contextAccessor) : ITargetingContextService
{
    public async Task<TargetingContext> GetTargetingContextAsync(CancellationToken ct = default)
    {
        HttpContext httpContext = contextAccessor.HttpContext!;
        var user = httpContext.User;
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return new()
            {
                UserId = "anonymous",
                Groups = []
            };
        }

        var distributionGroup = user.Claims.Single(c => c.Type == ClaimTypes.GroupSid).Value;
        return new()
        {
            UserId = user.Identity.Name,
            Groups = [distributionGroup]
        };
    }
}
