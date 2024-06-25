using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace API.Controllers;

[Route("api/ab-testing")]
[ApiController]
public class AbTestingController(IVariantFeatureManagerSnapshot featureManager) : ControllerBase
{
    [HttpGet("feature-flags")]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct = default)
    {
        var featureFlags = new Dictionary<string, string?>();
        var featureNames = featureManager.GetFeatureNamesAsync(ct).ToBlockingEnumerable(ct);
        foreach (var featureName in featureNames)
        {
            var variant = await featureManager.GetVariantAsync(featureName, ct).ConfigureAwait(false);
            featureFlags.Add(featureName, variant?.Configuration?.Get<string>());
        }
        return Ok(featureFlags);
    }
}
