using System.ComponentModel.DataAnnotations;
using static System.ArgumentNullException;
namespace IPSAG.AbTesting.ResponseModels;

/// <summary>
/// A/B Testing feature flag response model
/// </summary>
/// <param name="Name">Name of the feature flag</param>
/// <param name="Value">Value of the feature flag</param>
public record FeatureFlag([property: Required] string Name, string? Value, [property: Required] bool IsVariant)
{
    public static FeatureFlag Create(string name, bool value)
    {
        ThrowIfNull(name);
        return new(name, value.ToString(), false);
    }

    public static FeatureFlag CreateVariant(string name, string? value)
    {
        ThrowIfNull(name);
        return new(name, value, true);
    }
};
