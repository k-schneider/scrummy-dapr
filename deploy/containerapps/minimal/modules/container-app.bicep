param location string = resourceGroup().location
param containerAppName string
param environmentId string
param appInsightsName string

var gameServicePort = 3000

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
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
      dapr: {
        enabled: true
        appId: containerAppName
        appProtocol: 'http'
        appPort: gameServicePort
      }
    }
    template: {
      containers: [
        {
          image: 'docker.io/redis:alpine'
          name: 'redis'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          command: [
            '/bin/sh'
            '-c'
            'redis-server --requirepass "password" --save ""'
          ]
        }
        {
          image: 'ghcr.io/k-schneider/scrummy-dapr/game.service:main'
          name: 'game-service'
          args: [
            '--expose=${gameServicePort}'
          ]
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:${gameServicePort}'
            }
            {
              name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
              value: appInsights.properties.InstrumentationKey
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          probes: [
            {
              type: 'liveness'
              httpGet: {
                path: '/liveness'
                port: 80
              }
            }
            {
              type: 'readiness'
              httpGet: {
                path: '/hc'
                port: 80
              }
            }
          ]
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
              value: 'http://127.0.0.1:${gameServicePort}'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          probes: [
            {
              type: 'liveness'
              httpGet: {
                path: '/liveness'
                port: 80
              }
            }
            {
              type: 'readiness'
              httpGet: {
                path: '/hc'
                port: 80
              }
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}
