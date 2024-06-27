using Ips.AbTesting.Contexts;
using Ips.AbTesting.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Telemetry;
using Microsoft.FeatureManagement.Telemetry.ApplicationInsights.AspNetCore;

namespace Ips.AbTesting.Extensions;

public static class IServiceCollectionExtension
{
    public static void AddAbTesting(this IHostApplicationBuilder builder, string? appConfigConnectionString = null, string? appInsightsConnectionString = null)
    {
        builder.Configuration.AddAzureAppConfiguration(options
            => options.Connect(appConfigConnectionString ?? builder.Configuration.GetConnectionString("AppConfig"))
            .UseFeatureFlags(ffo =>
            {
                ffo.CacheExpirationInterval = TimeSpan.FromSeconds(5);
            }));

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAzureAppConfiguration()
            .AddFeatureManagement()
            .WithTargeting<TargetingContextAccessor>()
            .AddTelemetryPublisher<ApplicationInsightsTelemetryPublisher>();
        builder.Services.AddApplicationInsightsTelemetry(
            new ApplicationInsightsServiceOptions
            {
                ConnectionString = appInsightsConnectionString ?? builder.Configuration.GetConnectionString("AppInsights"),
                EnableAdaptiveSampling = false
            })
            .AddSingleton<ITelemetryInitializer, TargetingTelemetryInitializer>();
        //
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
    }

    public static void UseAbTesting(this IApplicationBuilder app)
    {
        app.UseAzureAppConfiguration();
        app.UseMiddleware<TargetingHttpContextMiddleware>();
        var appPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
        var partToRemove = appPartManager.ApplicationParts.FirstOrDefault(a => ((AssemblyPart)a).Assembly == typeof(IServiceCollectionExtension).Assembly);

        if (partToRemove != null)
        {
            appPartManager.ApplicationParts.Remove(partToRemove);
        }
    }

    public static IMvcBuilder AddAbTestingControllers(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(typeof(IServiceCollectionExtension).Assembly);
    }
}
