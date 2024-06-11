
param prefix string
param location string = resourceGroup().location

resource logWorkSpace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: '${prefix}-workspace'
  location: location
}

var primarySharedKey = logWorkSpace.listKeys().primarySharedKey
output workpsaceId string = logWorkSpace.id
output customerId string = logWorkSpace.properties.customerId
output sharedKey string = primarySharedKey
