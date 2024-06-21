using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace API.Controllers;

[Route("api/ab-testing")]
[ApiController]
public class AbTestingController : ControllerBase
{
    private readonly IVariantFeatureManagerSnapshot _featureManager;
    public AbTestingController(IVariantFeatureManagerSnapshot featureManager)
    {
        _featureManager = featureManager;
    }

    [HttpGet("feature-flags")]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct = default)
    {
        var featureFlags = new FeatureFlagModel();
        var properties = featureFlags.GetType().GetProperties();
        foreach (var property in properties)
        {
            var variant = await _featureManager.GetVariantAsync(property.Name, ct).ConfigureAwait(false);
            if (variant != null)
            {
                var value = variant.Configuration.Get<bool>();
                property.SetValue(featureFlags, value);
            }

        }
        return Ok(featureFlags);
    }
}
