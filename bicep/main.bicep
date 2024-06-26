targetScope = 'resourceGroup'

@allowed([
  'westeurope'
])
param location string = 'westeurope'
@secure()
param analyticsClarityId string
@secure()
param googleAnalyticsMesurementId string

var prefix = 'abtesting'
var resGroup = resourceGroup()
var keyVaultName = '${prefix}-key-vault'
module keyvault './General/keyvault.bicep' = {
  scope: resGroup
  name: 'keyvault-deployment'
  params: {
    keyVaultName: keyVaultName
    location: location
    tenantId: tenant().tenantId
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

module appConfig './General/app-config.bicep' = {
  name: 'deploy-app-config'
  scope: resGroup
  dependsOn: [keyvault]
  params: {
    keyVaultName: keyVaultName
    configStoreName: '${prefix}-app-configs'
    location: location
    appInsightsId: insights.outputs.resourceId
  }
}

module containerEnv './General/container-env.bicep' = {
  name: 'create-env'
  dependsOn: [logWorkSpace, insights]
  scope: resGroup
  params: {
    location: location
    prefix: prefix
    customerId: logWorkSpace.outputs.customerId
    instrumentationKey: insights.outputs.instrumentationKey
    primarySharedKey: logWorkSpace.outputs.sharedKey
  }
}

var secretNames = [insights.outputs.kvInstrumentationKey, appConfig.outputs.secretName, insights.outputs.kvConnection]
var envVariables = [
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    secretRef: insights.outputs.kvInstrumentationKey
  }
  {
    name: 'ApplicationInsights__InstrumentationKey'
    secretRef: insights.outputs.kvInstrumentationKey
  }
  {
    name: 'ANALYTICS_CLARITY_ID'
    value: analyticsClarityId
  }
  {
    name: 'ANALYTICS_GA_MEASUREMENT_ID'
    value: googleAnalyticsMesurementId
  }
  {
    name: 'ConnectionStrings__AppConfig'
    secretRef: appConfig.outputs.secretName
  }
  {
    name: 'ConnectionStrings__AppInsights'
    secretRef: insights.outputs.kvConnection
  }
]

module containerApp './General/container-app.bicep' = {
  name: 'web-container-deployment'
  scope: resGroup
  dependsOn: [containerEnv]
  params: {
    prefix: prefix
    location: location
    environmentId: containerEnv.outputs.id
    keyvaultName: keyVaultName
    secretNames: secretNames
    envVariables: envVariables
  }
}

module accessPolicy './Shared/add-keyvault-policy.bicep' = {
  name: 'add-kv-access-policies'
  scope: resGroup
  dependsOn: [containerApp]
  params: {
    keyVaultName: keyVaultName
    principalIds: [containerApp.outputs.principalId, '860ff5b0-134d-4d9b-8a18-1f996f3c52e1']
    tenantId: tenant().tenantId
  }
}
