param location string = resourceGroup().location

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: 'stContainerGroup'
  location: location
  properties: {
    containers: [
      {
        name: 'redis'
        properties: {
          image: 'abtestingplayground.azurecr.io/redis:latest'
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
          ports: [
            {
              port: 6379
            }
          ]
        
        }
      }
      {
        name: 'redis-commander'
        properties: {
          image: 'abtestingplayground.azurecr.io/rediscommander/redis-commander:latest'
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
          ports: [
            {
              port: 8081
            }
          ]
          environmentVariables: [
            {
              name: 'REDIS_HOSTS'
              value: 'local:redis:6379'
            }
          ]
        }
      }
      {
        name: 'api'
        properties: {
          image: 'abtestingplayground.azurecr.io/playground-api:latest'
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
          ports: [
            {
              port: 8080
            }
          ]
        }
      }
      {
        name: 'client'
        properties: {
          image: 'abtestingplayground.azurecr.io/playground-client:latest'
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
          ports: [
            {
              port: 80
            }
          ]
        }
      }
    ]
    osType: 'Linux'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: 'tcp'
          port: 6379
        }
        {
          protocol: 'tcp'
          port: 8081
        }
        {
          protocol: 'tcp'
          port: 8080
        }
        {
          protocol: 'tcp'
          port: 80
        }
      ]
    }
    restartPolicy: 'Always'
  }
  identity: {
    type: 'SystemAssigned'
  }
  
  // identity: [
  //   {
  //     server: 'abtestingplayground.azurecr.io'
  //     username: 'abtestingplayground'
  //     password: 'yxvUoXHlM1uqo8KpBqi0GPMPjpSgbh6wdMs1JMuXxD'
  //   }
  // ]
}

output test string = containerGroup.identity.principalId
