@description('Specifies the name of the key vault.')
param keyVaultName string
@description('Specifies secret name')
param secretName string
@description('Specifies secret value')
@secure()
param secretValue string

resource kv 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}
resource saSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {  
  parent: kv
  name: secretName
  properties: {
    value: secretValue
  }
}

output secretName string = saSecret.name
