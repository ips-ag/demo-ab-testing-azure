param prefix string
param environmentId string
param location string = resourceGroup().location
param secretNames array = []
param keyvaultName string
param envVariables object[] = []

var acrName ='abtesting'
resource acrResource 'Microsoft.ContainerRegistry/registries@2023-11-01-preview' = {
  name: acrName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}
var secrets = [
  for secret in secretNames: {
    name: secret
    keyVaultUrl: 'https://${keyvaultName}.vault.azure.net/secrets/${secret}'
    identity: 'system'
  }
]

resource containerApp 'Microsoft.App/containerApps@2023-11-02-preview' = {
  name: '${prefix}-playground'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      ingress: {
        targetPort: 80
        external: true
        
        // ipSecurityRestrictions: [
        //   {
        //     ipAddressRange: '193.86.239.130/32'
        //     action: 'Allow'
        //     name: 'Ips Prague'
        //   }
        //   {
        //     ipAddressRange: '45.122.237.234/32'
        //     action: 'Allow'
        //     name: 'Ips Danang'
        //   }
        // ]
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: [
        {
          server: acrResource.properties.loginServer
          username: acrResource.listCredentials().username
          passwordSecretRef: 'registry-password'
        }
      ]
      secrets: concat(
        [
          {
            name: 'registry-password'
            value: acrResource.listCredentials().passwords[0].value
          }
        ],
        secrets
      )
    }
    template: {
      containers: [
        {
          image: 'mcr.microsoft.com/k8se/quickstart:latest'
          name: 'playground'
          resources: {
            cpu: json('0.5')
            memory: '1.0Gi'
          }
          
          env: concat(
            [
              {
                name: 'WEBSITES_PORT'
                value: '80'
              }
            ],
            envVariables
          )
        }
      ]
      
      scale: {
        minReplicas: 0
        maxReplicas: 3
        rules: [
          {
            http: {
              metadata: {
                concurrentRequests: '100'
              }
            }
            name: 'http-usage'
          }
        ]
      }
    }
  }
}

output principalId string = containerApp.identity.principalId
