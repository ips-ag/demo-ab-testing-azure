@description('Specifies the name of the key vault.')
param keyVaultName string
@description('Specifies the location of the key vault.')
param location string
@allowed([
  'standard'
  'premium'
])
param skuName string = 'standard'
param tenantId string

resource kv 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: tenantId
    enableSoftDelete: true
    enabledForDeployment: true
    softDeleteRetentionInDays: 90
    enabledForTemplateDeployment: true
    createMode: 'recover'
    sku: {
      name: skuName
      family: 'A'
    }
  }
}
