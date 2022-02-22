param location string = resourceGroup().location
param resourceBaseName string
param environmentId string
param appInsightsName string

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: '${resourceBaseName}-app'
  kind: 'containerapp'
  location: location
  properties: {
    kubeEnvironmentId: environmentId
    configuration: {
      ingress: {
        external: true
        targetPort: 80
        allowInsecure: false
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
          name: 'redis'
          resources: {
            cpu: '0.5'
            memory: '1Gi'
          }
        }
        {
          image: 'ghcr.io/k-schneider/scrummy-dapr/game.service:main'
          name: 'game-service'
          args: [
            '--expose=3000'
          ]
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:3000'
            }
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
        {
          image: 'ghcr.io/k-schneider/scrummy-dapr/web.blazor:main'
          name: 'web-blazor'
          env: [
            {
              name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
              value: appInsights.properties.InstrumentationKey
            }
            {
              name: 'GameServiceUrl'
              value: 'http://localhost:3000'
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
        appPort: 3000
        appId: 'game-service-dapr'
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
