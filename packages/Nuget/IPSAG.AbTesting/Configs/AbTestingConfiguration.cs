using System.ComponentModel;

namespace IPSAG.AbTesting.Configs;

public class AbTestingConfiguration
{
    public string? AppConfigConnectionString { get; set; }
    public string? AppInsightsConnectionString { get; set; }
    [DefaultValue(true)]
    public bool UseDefaultControllers { get; set; } = true;
}
