targetScope = 'subscription'

@allowed([
  'westeurope'
])
param location string = 'westeurope'

var rgName = 'ab-testing-demo'
resource resGroup 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  location: location
  name: rgName
}

var keyVaultName = '${rgName}-vault'
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
  params: {
    prefix: rgName
    keyVaultName: keyVaultName
  }
}

module insights './General/insights.bicep' = {
  name: 'deploy-insights'
  scope: resGroup
  params: {
    location: location
    logAnaliticsWorkspaceId: logWorkSpace.outputs.workpsaceId
    prefix: rgName
    keyVaultName: keyVaultName
  }
}

module containerEnv './General/container-env.bicep' = {
  name: 'create-env'
  scope: resGroup
  params: {
    location: location
    prefix: rgName
  }
}

var secretNames = [insights.outputs.kvInstrumentationKey]
var envVariables = [
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    secretRef: insights.outputs.kvInstrumentationKey
  }
  {
    name: 'ApplicationInsights__InstrumentationKey'
    secretRef:  insights.outputs.kvInstrumentationKey
  }
  {
    name: 'ConnectionStrings__DefaultConnection'
    secretRef: 'TODO'
  }
  {
    name: 'ConnectionStrings__IdentityConnection'
    secretRef: 'TODO'
  }
  {
    name: 'ConnectionStrings__Redis'
    secretRef: 'TODO'
  }
]

module webAppApi './General/container-app.bicep' = {
  name: 'web-container-deployment'
  scope: resGroup
  params: {
    prefix: rgName
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
  params: {
    keyVaultName: keyVaultName
    principalIds: [webAppApi.outputs.principalId]
    tenantId: tenant().tenantId
  }
}
