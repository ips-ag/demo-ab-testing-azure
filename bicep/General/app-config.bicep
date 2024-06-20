@description('Specifies the name of the App Configuration store.')
param configStoreName string

@description('Specifies the Azure location where the app configuration store should be created.')
param location string = resourceGroup().location

@description('Specifies the names of the key-value resources. The name is a combination of key and label with $ as delimiter. The label is optional.')
param keyValueNames array = [
  'myKey'
  'myKey$myLabel'
]

@description('Specifies the values of the key-value resources. It\'s optional')
param keyValueValues array = [
  'Key-value without label'
  'Key-value with label'
]

@description('Specifies the content type of the key-value resources. For feature flag, the value should be application/vnd.microsoft.appconfig.ff+json;charset=utf-8. For Key Value reference, the value should be application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8. Otherwise, it\'s optional.')
param contentType string = 'the-content-type'

@description('Adds tags for the key-value resources. It\'s optional')
param tags object = {
  tag1: 'tag-value-1'
  tag2: 'tag-value-2'
}
@description('Specifies the name of the key vault.')
param keyVaultName string

resource configStore 'Microsoft.AppConfiguration/configurationStores@2023-09-01-preview' = {
  name: configStoreName
  location: location
  sku: {
    name: 'free'
  }
}

resource configStoreKeyValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = [
  for (item, i) in keyValueNames: {
    parent: configStore
    name: item
    properties: {
      value: keyValueValues[i]
      contentType: contentType
      tags: tags
    }
  }
]

var readonlyKey = filter(configStore.listKeys().value, k => k.name == 'Primary Read Only')[0]

module addKey '../Shared/add-keyvault-secret.bicep' = {
  name: 'add-app-config-cs'
  scope: resourceGroup()
  params: {
    keyVaultName: keyVaultName
    secretName: 'app-config-cs'
    secretValue: readonlyKey.connectionString
  }
}

output secretName string = addKey.outputs.secretName
