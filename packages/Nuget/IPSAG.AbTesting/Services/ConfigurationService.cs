using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace IPSAG.AbTesting.Services;

public interface IConfigurationService
{
    Task<IReadOnlyDictionary<string, string?>> ReadFeatureFlagsAsync(CancellationToken ct = default);
}

internal class ConfigurationService(IVariantFeatureManagerSnapshot featureManager) : IConfigurationService
{
    public async Task<IReadOnlyDictionary<string, string?>> ReadFeatureFlagsAsync(CancellationToken ct = default)
    {
        var featureFlags = new Dictionary<string, string?>();
        var featureNames = featureManager.GetFeatureNamesAsync(ct).ToBlockingEnumerable(ct);
        foreach (var featureName in featureNames)
        {
            var variant = await featureManager.GetVariantAsync(featureName, ct).ConfigureAwait(false);
            featureFlags.Add(featureName, variant != null ? variant.Configuration.Get<string?>() : (await featureManager.IsEnabledAsync(featureName, ct)).ToString());
        }
        return featureFlags;
    }
}
