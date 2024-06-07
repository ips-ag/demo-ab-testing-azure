@description('Specifies the name of the key vault.')
param keyVaultName string
@description('Specifies the location of the key vault.')
param location string
@allowed([
  'standard'
  'premium'
])
param skuName string = 'standard'
@description('Specifies the list of principalsto access this keyvault')
param principalIds array = []
param tenantId string

resource kv 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: tenantId
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    accessPolicies: [for i in principalIds: {
      objectId: i
      tenantId: tenantId
      permissions: {
        secrets: [
          'get'
        ]
      }
    }]
    sku: {
      name: skuName
      family: 'A'
    }
  }
}
