param prefix string
param location string = resourceGroup().location

resource logWorkSpace 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: '${prefix}-workspace'
}

resource azAppInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: '${prefix}-insights'
}

resource environment 'Microsoft.App/managedEnvironments@2023-11-02-preview' = {
  name: '${prefix}-app-env'
  location: location
  properties: {
    daprAIInstrumentationKey: azAppInsights.properties.InstrumentationKey
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logWorkSpace.properties.customerId
        sharedKey: logWorkSpace.listKeys().primarySharedKey
      }
    }
  }
}

output id string = environment.id
output name string = environment.name
