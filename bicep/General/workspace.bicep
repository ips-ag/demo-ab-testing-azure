
param prefix string
param location string = resourceGroup().location
@description('Specifies the name of the key vault.')
param keyVaultName string

resource logWorkSpace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: '${prefix}-workspace'
  location: location
}

module addCustomerId '../Shared/add-keyvault-secret.bicep' = {
  name: 'add-workspace-customerId'
  scope: resourceGroup()
  params: {
    keyVaultName: keyVaultName
    secretName: 'workspace-customerId'
    secretValue: logWorkSpace.properties.customerId
  }
}

module addSharedKey '../Shared/add-keyvault-secret.bicep' = {
  name: 'add-workspace-shared-key'
  scope: resourceGroup()
  params: {
    keyVaultName: keyVaultName
    secretName: 'workspace-shared-key'
    secretValue: logWorkSpace.listKeys().primarySharedKey
  }
}

output workpsaceId string = logWorkSpace.id
output kvCustomerId string = addCustomerId.outputs.secretName
output kvSharedKey string = addSharedKey.outputs.secretName
