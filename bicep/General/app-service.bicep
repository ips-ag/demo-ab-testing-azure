param prefix string = uniqueString(resourceGroup().id) // Generate unique String for web app name
param sku string = 'F1' // The SKU of App Service Plan
param linuxFxVersion string = 'DOTNETCORE|7.0' // The runtime stack of web app
param location string = resourceGroup().location // Location for all resources
param azAppInsightsInstrumentationKey string
param azAppInsightsConnectionString string
var webSiteName = toLower('${prefix}-app')
var appServicePlanName = toLower('asp-${webSiteName}')

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServicePlanName
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: sku
  }
  kind: 'linux'
}

resource appService 'Microsoft.Web/sites@2020-06-01' = {
  name: webSiteName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: linuxFxVersion
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource acrResource 'Microsoft.ContainerRegistry/registries@2023-11-01-preview' = {
  name: '${prefix}playground'
  location: resourceGroup().location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}
var credential = acrResource.listCredentials()

var BASE_APP_SETTINGS = {
  APPINSIGHTS_INSTRUMENTATIONKEY: azAppInsightsInstrumentationKey
  APPLICATIONINSIGHTS_CONNECTION_STRING: azAppInsightsConnectionString
  APPINSIGHTS_PROFILERFEATURE_VERSION: '1.0.0'
  APPINSIGHTS_SNAPSHOTFEATURE_VERSION: '1.0.0'
  ApplicationInsightsAgent_EXTENSION_VERSION: '~2'
  DOCKER_REGISTRY_SERVER_URL:  acrResource.properties.loginServer
  DOCKER_REGISTRY_SERVER_USERNAME: credential.username
  DOCKER_REGISTRY_SERVER_PASSWORD: first(credential.passwords).value
}

resource webAppSettings 'Microsoft.Web/sites/config@2022-09-01' = {
  parent: appService
  name: 'appsettings'
  kind: 'string'
  properties: BASE_APP_SETTINGS
}

output principalIds array = [appService.identity.principalId]
