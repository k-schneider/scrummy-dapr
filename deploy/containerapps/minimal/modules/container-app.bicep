param location string = resourceGroup().location
param containerAppName string
param environmentId string
param appInsightsName string

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
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
        appPort: 3000
      }
    }
    template: {
      containers: [
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
              value: 'http://localhost:3000'
            }
          ]
          resources: {
            cpu: '0.5'
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
        minReplicas: 0
        maxReplicas: 1
      }
    }
  }
}
