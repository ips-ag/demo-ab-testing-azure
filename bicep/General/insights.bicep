param location string = resourceGroup().location
param prefix string
param logAnaliticsWorkspaceId string
@description('Specifies the name of the key vault.')
param keyVaultName string

resource azAppInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${prefix}-insights'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnaliticsWorkspaceId
  }
}

module addKey '../Shared/add-keyvault-secret.bicep' = {
  name: 'add-insights-instrumentation-key'
  scope: resourceGroup()
  params: {
    keyVaultName: keyVaultName
    secretName: 'insights-instrumentation-key'
    secretValue: azAppInsights.properties.InstrumentationKey
  }
}
output resourceId string = azAppInsights.id
output kvInstrumentationKey string = addKey.outputs.secretName
output instrumentationKey string = azAppInsights.properties.InstrumentationKey

