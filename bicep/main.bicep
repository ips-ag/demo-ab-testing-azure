targetScope = 'resourceGroup'

@allowed([
  'westeurope'
])
param location string = 'westeurope'

var prefix = 'abtesting'
var resGroup = resourceGroup()
var keyVaultName = 'kv-${prefix}'
module keyvault './General/keyvault.bicep' = {
  scope: resGroup
  name: 'keyvault-deployment'
  params: {
    keyVaultName: keyVaultName
    location: location
    tenantId: tenant().tenantId
    principalIds: []
  }
}

module logWorkSpace './General/workspace.bicep' = {
  name: 'deploy-workspace'
  scope: resGroup
  dependsOn: [keyvault]
  params: {
    prefix: prefix
  }
}

module insights './General/insights.bicep' = {
  name: 'deploy-insights'
  scope: resGroup
  dependsOn: [keyvault]
  params: {
    location: location
    logAnaliticsWorkspaceId: logWorkSpace.outputs.workpsaceId
    prefix: prefix
    keyVaultName: keyVaultName
  }
}

module webAppApi './General/app-service.bicep' = {
  name: 'deploy-app-service'
  dependsOn: [insights]
  scope: resGroup
  params: {
    azAppInsightsConnectionString: insights.outputs.connectionString
    azAppInsightsInstrumentationKey: insights.outputs.instrumentationKey
    prefix: prefix
    linuxFxVersion: 'COMPOSE|${loadFileAsBase64('./docker-compose.yml')}'
    location: location
    sku: 'F1'
  }
}

module accessPolicy './Shared/add-keyvault-policy.bicep' = {
  name: 'add-kv-access-policies'
  scope: resGroup
  dependsOn: [webAppApi]
  params: {
    keyVaultName: keyVaultName
    principalIds: webAppApi.outputs.principalIds
    tenantId: tenant().tenantId
  }
}
