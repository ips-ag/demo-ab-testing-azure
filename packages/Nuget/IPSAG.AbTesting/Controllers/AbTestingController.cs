using IPSAG.AbTesting.Services;
using Microsoft.AspNetCore.Mvc;

namespace IPSAG.AbTesting.Controllers;

[Route("api/ab-testing")]
[ApiController]
public class AbTestingController(IConfigurationService abTestingConfigService) : ControllerBase
{
    [HttpGet("feature-flags")]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct = default)
    {
        var featureFlags = await abTestingConfigService.ReadFeatureFlagsAsync(ct).ConfigureAwait(false);
        return Ok(featureFlags);
    }
}
