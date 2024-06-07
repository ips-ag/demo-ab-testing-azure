@description('Specifies the name of the key vault.')
param keyVaultName string
@description('Specifies the list of principalsto access this keyvault')
param principalIds array = []
param tenantId string

resource kv 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}
resource symbolicname 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  name:  'add'
  parent: kv
  properties: {

    accessPolicies: [for i in principalIds: {
      objectId: i
      tenantId: tenantId
      permissions: {
        secrets: [
          'get'
        ]
      }
    }]
  }  
}
