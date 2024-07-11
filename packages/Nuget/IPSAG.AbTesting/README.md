# IPSAG.AbTesting library for .NET
## Introduction
The A/B Testing Nuget is a .NET-based library designed to facilitate A/B testing experiments in a controlled environment. It should be included together with an npm package to complete the integration, see example in the playground.

* [playground][playground]         | [Source code][playground_source]  | [README][playground_readme]
* [IPSAG.AbTesting][package]       | [Source code][source]             | [README][source_readme]
* [@ips-ag/abtesting][npm_package] | [Source code][npm_source]         | [README][npm_readme]

## Getting Started

### Install the package

Install the Azure Storage Blobs client library for .NET with [NuGet][nuget]:

```dotnetcli
dotnet add package IPSAG.AbTesting
```

### Prerequisites
You need an [Azure subscription][azure_sub] and a [App Configuration][app_config_docs] to use this package. The [App Insight][app_insights_docs] is not mandatory.

To create a new App Configuration, you can use the [Azure Portal][app_config_create_portal], or the [Azure CLI][app_config_create_cli].
Here's an example using the Azure CLI:
```Powershell
az appconfig create --name MyAppConfig --resource-group MyResourceGroup --location westus
```
Before you can use the [IPSAG.AbTesting][package], ensure you have the following prerequisites installed on your system:

- **.NET 8.0 or higher SDK Application**: Required to install the Nuget. You can download it from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

With these prerequisites in place, you're ready to start using the A/B Testing libraries to conduct your A/B testing experiments.

### How to use

In order to interact with the A/B Testing service, you'll need to configure connection to Azure App Configuration.

```C# 
// Create Host builder
var builder = WebApplication.CreateBuilder(args);
// Sample connection strings
var (appConfigCs, appInsightsCs) = ("<AppConfigConnectionString>", "<AppInsightsConnectionString>");
```
```C# 
// Configure A/B Testing by providing connection to App Configuration & App Insights
// By options
builder.AddAbTesting<TargetingContextService>(options =>
{
    options.AppConfigConnectionString = appConfigCs;
    options.AppInsightsConnectionString = appInsightsCs;
    options.UseDefaultControllers = true; // default value
});
// By options & configuration
builder.AddAbTesting<TargetingContextService>((options, configuration) =>
{
    options.AppConfigConnectionString = configuration.GetConnectionString("AppConfig");
    options.AppInsightsConnectionString = configuration.GetConnectionString("AppInsights");
    options.UseDefaultControllers = true; // default value
});
// OR by parameters
builder.AddAbTesting<TargetingContextService>(appConfigCs!,
                                              appInsightsCs);
```
```C# 
// Configure A/B Testing by providing connection to App Configuration only
//By options
builder.AddAbTesting<TargetingContextService>(options =>
{
    options.AppConfigConnectionString = appConfigCs;
    options.UseDefaultControllers = true; // default value
});
// By options & configuration
builder.AddAbTesting<TargetingContextService>((options, configuration) =>
{
    options.AppConfigConnectionString = configuration.GetConnectionString("AppConfig");
    options.UseDefaultControllers = true; // default value
});
// OR by parameters
builder.AddAbTesting<TargetingContextService>(appConfigCs!);
```
```C# 
var app = builder.Build();
// If you are using Authorization information to build the TargetingContext,
// please ensure that app.UseAbTesting() was placed after app.UseAuthentication(); & app.UseAuthorization();
app.UseAbTesting();
//
```
The `TargetingContextService` is an implementation from the interface `IPSAG.AbTesting.Services.ITargetingContextService` which is required to use A/B Testing package.

There, you could define your own logic to create the `TargetingContext` which can reflect to App Configuration
```C# 
// Example
public class TargetingContextService(IHttpContextAccessor contextAccessor) : ITargetingContextService
{
    public async Task<TargetingContext> GetTargetingContextAsync(CancellationToken ct = default)
    {
        HttpContext httpContext = contextAccessor.HttpContext!;
        var user = httpContext.User;
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return new()
            {
                UserId = "anonymous",
                Groups = []
            };
        }

        var distributionGroup = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GroupSid)?.Value;
        return new()
        {
            UserId = user.Identity.Name,
            Groups = string.IsNullOrEmpty(distributionGroup) ? [] : [distributionGroup]
        };
    }
}
```

By default, a default AbTestingController would be injected to your application

``` C# 
[Route("api/ab-testing")]
public class AbTestingController(IConfigurationService abTestingConfigService) : BaseApiController
{
    [HttpGet("feature-flags")]
    [ProducesResponseType(typeof(FeatureFlag[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct = default)
    {
        var featureFlags = await abTestingConfigService.ReadFeatureFlagsAsync(ct).ConfigureAwait(false);
        return featureFlags.Match(Ok, ServerError);
    }
}
```

Request sample
```bat
curl -X 'GET' \
  'http://localhost:5103/api/ab-testing/feature-flags' \
  -H 'accept: application/json'
```
Response body
``` JSON
[
  {
    "name": "ShopFilter",
    "value": "False",
    "isVariant": true
  },
  {
    "name": "ShopFilterEarlyAccess",
    "value": "False",
    "isVariant": false
  },
  {
    "name": "ShopFilterVersion",
    "value": "0.0.1",
    "isVariant": true
  }
]
```

But you could also use your own controllers by removing the default controllers and defining your controllers and injecting IConfigurationService

``` C# 
// Remove default controllers
builder.AddAbTesting<TargetingContextService>(options =>
{
    options.AppConfigConnectionString = appConfigCs;
    options.AppInsightsConnectionString = appInsightsCs;
    options.UseDefaultControllers = false; // <-- set to false
});
....
// Create new controller
[Route("api/my-route")]
public class MyController(IConfigurationService abTestingConfigService)
{
    // Implementation
}
```

## Related Packages
* [JetBrains.Annotations](https://www.nuget.org/packages/JetBrains.Annotations) (>= 2024.2.0)
* [Microsoft.Azure.AppConfiguration.AspNetCore](https://www.nuget.org/packages/Microsoft.Azure.AppConfiguration.AspNetCore/8.0.0-preview.2) (>= 8.0.0-preview.2)
* [Microsoft.FeatureManagement.AspNetCore](https://www.nuget.org/packages/Microsoft.FeatureManagement.AspNetCore/4.0.0-preview3) (>= 4.0.0-preview3)
* [Microsoft.FeatureManagement.Telemetry.ApplicationInsights](https://www.nuget.org/packages/Microsoft.FeatureManagement.Telemetry.ApplicationInsights/4.0.0-preview3) (>= 4.0.0-preview3)
* [Microsoft.FeatureManagement.Telemetry.ApplicationInsights.AspNetCore](https://www.nuget.org/packages/Microsoft.FeatureManagement.Telemetry.ApplicationInsights.AspNetCore/4.0.0-preview3) (>= 4.0.0-preview3)

## License

* The `IPSAG.AbTesting` NuGet package and its source code under the [`packages/Nuget` directory][source] are distributed under the terms of the [MIT License][license].

## Feedback

We value your feedback! If you have any suggestions, bug reports, or feature requests, please open an issue on our GitHub repository.


<!-- LINKS -->
[source]: https://github.com/ips-ag/demo-ab-testing-azure/tree/main/packages/Nuget/IPSAG.AbTesting
[npm_source]: https://github.com/ips-ag/demo-ab-testing-azure/tree/main/packages/npm/abtesting
[playground_source]: https://github.com/ips-ag/demo-ab-testing-azure/tree/main/playground
[source_readme]: https://github.com/ips-ag/demo-ab-testing-azure/tree/main/packages/Nuget/IPSAG.AbTesting/README.md
[npm_readme]: https://github.com/ips-ag/demo-ab-testing-azure/tree/main/packages/npm/abtesting/README.md
[playground_readme]: https://github.com/ips-ag/demo-ab-testing-azure/tree/main/playground/README.md
[package]: https://www.nuget.org/packages/IPSAG.AbTesting/
[nuget]: https://www.nuget.org/
[npm_package]: https://www.npmjs.com/package/@ips-ag/abtesting
[playground]: https://github.com/ips-ag/demo-ab-testing-azure/tree/main/playground
[IPS]: https://www.ips-ag.com
[azure_sub]: https://azure.microsoft.com/free/dotnet/
[app_config_docs]: https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview
[app_config_create_portal]: https://learn.microsoft.com/en-us/azure/azure-app-configuration/quickstart-azure-app-configuration-create?tabs=azure-portal
[app_config_create_cli]: https://learn.microsoft.com/en-us/azure/azure-app-configuration/quickstart-azure-app-configuration-create?tabs=azure-cli
[app_insights_docs]: https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview
[license]: https://github.com/ips-ag/demo-ab-testing-azure/blob/main/LICENSE
[nuget_icon]: https://www.nuget.org/Content/gallery/img/default-package-icon-256x256.png
[npm_icon]: https://static-production.npmjs.com/7a7ffabbd910fc60161bc04f2cee4160.png