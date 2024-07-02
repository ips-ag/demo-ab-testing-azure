using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace IPS.AbTesting.Services;

public interface IConfigurationService
{
    Task<IReadOnlyDictionary<string, dynamic>> ReadFeatureFlagsAsync(CancellationToken ct = default);
}

internal class ConfigurationService(IVariantFeatureManagerSnapshot featureManager) : IConfigurationService
{
    public async Task<IReadOnlyDictionary<string, dynamic>> ReadFeatureFlagsAsync(CancellationToken ct = default)
    {
        var featureFlags = new Dictionary<string, dynamic>();
        var featureNames = featureManager.GetFeatureNamesAsync(ct).ToBlockingEnumerable(ct);
        foreach (var featureName in featureNames)
        {
            var variant = await featureManager.GetVariantAsync(featureName, ct).ConfigureAwait(false);
            featureFlags.Add(featureName, variant != null ? variant.Configuration.Get<dynamic>() : await featureManager.IsEnabledAsync(featureName, ct));
        }
        return featureFlags;
    }
}
