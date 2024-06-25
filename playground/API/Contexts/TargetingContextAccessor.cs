using Microsoft.FeatureManagement.FeatureFilters;

namespace API.Contexts;

public class TargetingContextAccessor(IHttpContextAccessor httpContextAccessor) : ITargetingContextAccessor
{
    private const string TargetingContextLookup = "TargetingContextAccessor.TargetingContext";

    public ValueTask<TargetingContext> GetContextAsync()
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;
        if (httpContext.Items.TryGetValue(TargetingContextLookup, out var value))
        {
            return new ValueTask<TargetingContext>((TargetingContext)value!);
        }
        List<string> groups = [];
        if (httpContext.User?.Identity?.Name != null)
        {
            groups.Add(httpContext.User.Identity.Name.Split("@", StringSplitOptions.None)[1]);
        }
        TargetingContext targetingContext = new()
        {
            //UserId = httpContext.User.Identity.Name ?? "guest",
            // TODO
            UserId = Guid.NewGuid().ToString(),
            Groups = groups
        };
        httpContext.Items[TargetingContextLookup] = targetingContext;
        return new ValueTask<TargetingContext>(targetingContext);
    }
}
