using IPSAG.AbTesting.ResponseModels;
using IPSAG.AbTesting.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPSAG.AbTesting.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="abTestingConfigService"></param>
[Route("api/ab-testing")]
public class AbTestingController(IConfigurationService abTestingConfigService) : BaseApiController
{
    /// <summary>
    /// Get all availabe feature flags
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("feature-flags")]
    [ProducesResponseType(typeof(FeatureFlag[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct = default)
    {
        var featureFlags = await abTestingConfigService.ReadFeatureFlagsAsync(ct).ConfigureAwait(false);
        return featureFlags.Match(Ok, ServerError);
    }
}
