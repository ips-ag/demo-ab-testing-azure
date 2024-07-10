using IPSAG.AbTesting.Monads;
using IPSAG.AbTesting.ResponseModels;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace IPSAG.AbTesting.Services;

public interface IConfigurationService
{
    Task<Maybe<FeatureFlag[]>> ReadFeatureFlagsAsync(CancellationToken ct = default);
}

internal class ConfigurationService(IVariantFeatureManagerSnapshot featureManager) : IConfigurationService
{
    public async Task<Maybe<FeatureFlag[]>> ReadFeatureFlagsAsync(CancellationToken ct = default)
    {
        try
        {
            var featureNames = featureManager.GetFeatureNamesAsync(ct).ToBlockingEnumerable(ct);
            var getFlagValues = featureNames.Select(async featureName =>
            {
                var variant = await featureManager.GetVariantAsync(featureName, ct).ConfigureAwait(false);
                if (variant is null)
                {
                    var flagValue = await featureManager.IsEnabledAsync(featureName, ct).ConfigureAwait(false);
                    return FeatureFlag.Create(featureName, flagValue);
                }
                else
                {
                    var flagValue = variant.Configuration.Get<string?>();
                    return FeatureFlag.CreateVariant(featureName, flagValue);
                }
            });

            var featureFlags = await Task.WhenAll(getFlagValues).ConfigureAwait(false);
            return Maybe<FeatureFlag[]>.Some(featureFlags);
        }
        catch (Exception ex)
        {
            return Maybe<FeatureFlag[]>.Error(ex);
        }
    }
}
