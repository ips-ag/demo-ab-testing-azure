using IPSAG.AbTesting.Configs;
using IPSAG.AbTesting.Contexts;
using IPSAG.AbTesting.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Telemetry;
using Microsoft.FeatureManagement.Telemetry.ApplicationInsights.AspNetCore;
using static System.ArgumentException;

namespace IPSAG.AbTesting.Extensions;

/// <summary>
/// 
/// </summary>
public static class IServiceCollectionExtension
{
    public static void AddAbTesting<TTargetingContextService>(this IHostApplicationBuilder builder,
                                                                  Action<AbTestingConfiguration> configure)
        where TTargetingContextService : class, ITargetingContextService
            => builder.AddAbTesting<TTargetingContextService>((o, c) => configure(o));

    public static void AddAbTesting<TTargetingContextService>(this IHostApplicationBuilder builder,
                                                          string appConfigConnectionString,
                                                          string? appInsightsConnectionString = null)
        where TTargetingContextService : class, ITargetingContextService
             => builder.AddAbTesting<TTargetingContextService>((o, c) =>
             {
                 o.AppConfigConnectionString = appConfigConnectionString;
                 o.AppInsightsConnectionString = appInsightsConnectionString;
             });

    public static void AddAbTesting<TTargetingContextService>(this IHostApplicationBuilder builder,
                                                              Action<AbTestingConfiguration, IConfiguration> configure,
                                                              string configSection = nameof(AbTestingConfiguration))
        where TTargetingContextService : class, ITargetingContextService
    {
        var services = builder.Services;
        services.AddOptions<AbTestingConfiguration>().Configure<IConfiguration>((o, c) =>
        {
            c.GetSection(configSection).Bind(o);
            configure!.Invoke(o, c);
        });
        //
        var serviceProvider = services.BuildServiceProvider();
        var configOptions = serviceProvider.GetOptionsValue<AbTestingConfiguration>(nameof(AbTestingConfiguration.AppConfigConnectionString));
        builder.Configuration.AddAzureAppConfiguration(options
            => options.Connect(configOptions.AppConfigConnectionString)
            .UseFeatureFlags(ffo =>
            {
                ffo.CacheExpirationInterval = TimeSpan.FromSeconds(5);
            }));

        builder.Services.AddHttpContextAccessor();
        var featureBuilder = builder.Services.AddAzureAppConfiguration()
            .AddFeatureManagement()
            .WithTargeting<TargetingContextAccessor>();
        if (!string.IsNullOrEmpty(configOptions.AppInsightsConnectionString))
        {
            featureBuilder.AddTelemetryPublisher<ApplicationInsightsTelemetryPublisher>();
            builder.Services.AddApplicationInsightsTelemetry(
            new ApplicationInsightsServiceOptions
            {
                ConnectionString = configOptions.AppInsightsConnectionString,
                EnableAdaptiveSampling = false
            })
            .AddSingleton<ITelemetryInitializer, TargetingTelemetryInitializer>();
        }
        //
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
        builder.Services.AddSingleton<ITargetingContextService, TTargetingContextService>();
    }

    public static IApplicationBuilder UseAbTesting(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetOptionsValue<AbTestingConfiguration>();
        if (!configuration.UseDefaultControllers)
        {
            var appPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            var partToRemove = appPartManager.ApplicationParts.FirstOrDefault(a => ((AssemblyPart)a).Assembly == typeof(IServiceCollectionExtension).Assembly);

            if (partToRemove != null)
            {
                appPartManager.ApplicationParts.Remove(partToRemove);
            }
        }
        return app.UseAzureAppConfiguration().UseMiddleware<TargetingHttpContextMiddleware>();

    }

    public static IMvcBuilder ConfigureApiBehavior(this IMvcBuilder builder)
    {
        return builder.ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressMapClientErrors = true;
        });
    }
    #region Internal

    internal static TData GetValue<TData>(this IOptions<TData> options, params string[] requiredParams) where TData : class, new()
    {
        var config = options.Value;
        foreach (var item in requiredParams)
        {
            var (PropertyName, _, PropertyValue) = config.GetPropertyValue(item);
            ThrowIfNullOrEmpty(PropertyValue?.ToString(), PropertyName);
        }

        return config;
    }

    internal static TData GetOptionsValue<TData>(this IServiceProvider provider, params string[] requiredParams) where TData : class, new()
    {
        var configOptions = provider.GetRequiredService<IOptions<TData>>();
        return configOptions.GetValue(requiredParams);
    }

    private static (string PropertyName, bool PropertyExists, object? PropertyValue) GetPropertyValue<TData>(this TData? obj, string propertyName) where TData : class
    {
        if (obj == null)
            return (propertyName, false, null);
        //
        var objectType = obj.GetType();
        var prop = objectType.GetProperty(propertyName);
        var propName = $"{objectType.Name}.{propertyName}";
        if (prop != null)
            return (propName, true, prop.GetValue(obj));
        //
        var properties = obj.GetType().GetProperties();
        var nestedProperties = properties.Where(p => p.PropertyType.IsClass && !p.PropertyType.IsValueType);
        foreach (var property in nestedProperties)
        {
            var propValue = property.GetValue(obj);
            if (propValue is not string)
            {
                (var nestedPropName, var propExists, var nestedPropValue) = property.GetValue(obj).GetPropertyValue(propertyName);
                if (propExists)
                {
                    return ($"{objectType.Name}.{nestedPropName}", propExists, nestedPropValue);
                }
            }
        }

        return (propName, false, null);
    }

    #endregion
}
