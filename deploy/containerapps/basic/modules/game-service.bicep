param location string = resourceGroup().location
param resourceBaseName string
param environmentId string
param appInsightsName string

var containerAppName = '${resourceBaseName}-game-service'
var containerPort = 80

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: containerAppName
  kind: 'containerapp'
  location: location
  properties: {
    kubeEnvironmentId: environmentId
    configuration: {
      ingress: {
        external: false
        targetPort: containerPort
        allowInsecure: true
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      secrets: [
        {
          name: 'redis-password'
          value: ''
        }
      ]
    }
    template: {
      containers: [
        {
          image: 'redis:alpine'
          name: '${resourceBaseName}-redis'
          resources: {
            cpu: '0.5'
            memory: '1Gi'
          }
        }
        {
          image: 'ghcr.io/k-schneider/scrummy-dapr/game.service:main'
          name: containerAppName
          env: [
            {
              name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
              value: appInsights.properties.InstrumentationKey
            }
          ]
          resources: {
            cpu: '0.5'
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
      dapr: {
        enabled: true
        appPort: containerPort
        appId: '${containerAppName}-dapr'
        components: [
          {
            name: 'statestore'
            type: 'state.redis'
            version: 'v1'
            metadata: [
              {
                name: 'redisHost'
                value: 'localhost:6379'
              }
              {
                name: 'redisPassword'
                secretRef: 'redis-password'
              }
              {
                name: 'actorStateStore'
                value: 'true'
              }
            ]
          }
          {
            name: 'pubsub'
            type: 'pubsub.redis'
            version: 'v1'
            metadata: [
              {
                name: 'redisHost'
                value: 'localhost:6379'
              }
              {
                name: 'redisPassword'
                secretRef: 'redis-password'
              }
            ]
          }
        ]
      }
    }
  }
}

output containerAppName string = containerApp.name
output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn
