targetScope = 'resourceGroup'

@allowed([
  'westeurope'
])
param location string = 'westeurope'

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

// module sqlSever './General/sql.bicep' = {
//   name: 'deploy-sql'
//   dependsOn: [keyvault]
//   scope: resGroup
//   params: {
//     administratorLogin: 'abtesting'
//     administratorLoginPassword: 'abtestingdemo'
//     location: location
//     sqlDBName: 'ABTestingDemo'
//     keyVaultName: keyVaultName
//   }
// }

var secretNames = [insights.outputs.kvInstrumentationKey]
var envVariables = [
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    secretRef: insights.outputs.kvInstrumentationKey
  }
  {
    name: 'ApplicationInsights__InstrumentationKey'
    secretRef: insights.outputs.kvInstrumentationKey
  }
  // {
  //   name: 'ConnectionStrings__DefaultConnection'
  //   secretRef: sqlSever.outputs.kvConnectionString
  // }
  // {
  //   name: 'ConnectionStrings__IdentityConnection'
  //   secretRef: sqlSever.outputs.kvConnectionString
  // }
  // {
  //   name: 'ConnectionStrings__Redis'
  //   secretRef: 'TODO'
  // }
]

module webAppApi './General/container-app.bicep' = {
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
  dependsOn: [webAppApi]
  params: {
    keyVaultName: keyVaultName
    principalIds: [webAppApi.outputs.principalId]
    tenantId: tenant().tenantId
  }
}
