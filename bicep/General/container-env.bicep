param prefix string
param location string = resourceGroup().location
@secure()
param instrumentationKey string
@secure()
param primarySharedKey string
@secure()
param customerId string

resource environment 'Microsoft.App/managedEnvironments@2023-11-02-preview' = {
  name: '${prefix}-app-env'
  location: location
  properties: {
    daprAIInstrumentationKey: instrumentationKey
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: customerId
        sharedKey: primarySharedKey
      }
    }
  }
}

output id string = environment.id
output name string = environment.name
