@description('The name of the SQL logical server.')
param serverName string = uniqueString('sql', resourceGroup().id)

@description('The name of the SQL Database.')
param sqlDBName string = 'SampleDB'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('The administrator username of the SQL logical server.')
param administratorLogin string

@description('The administrator password of the SQL logical server.')
@secure()
param administratorLoginPassword string
param keyVaultName string

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: serverName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDBName
  location: location
  sku: {
    name: 'Free'
    tier: 'Free'
  }
}

var connectionString = 'Server=tcp:${reference(serverName).fullyQualifiedDomainName},1433;Initial Catalog=${sqlDBName};Persist Security Info=False;User ID=${reference(serverName).administratorLogin};Password=${reference(serverName).administratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
module addSqlConnection '../Shared/add-keyvault-secret.bicep' = {
  name: 'add-workspace-customerId'
  scope: resourceGroup()
  params: {
    keyVaultName: keyVaultName
    secretName: 'sql-connection'
    secretValue: connectionString
  }
}

output kvConnectionString string = 'sql-connection'
