param location string = resourceGroup().location
param containerAppName string
param environmentId string
param containerRegistry string
param containerImage string
param gameServiceFqdn string
param appInsightsName string
param cpuCore string
param memorySize string
param minReplicas int
param maxReplicas int
param scaleConcurrentRequests string

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
    }
    template: {
      containers: [
        {
          image: '${containerRegistry}/${containerImage}'
          name: containerAppName
          env: [
            {
              name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
              value: appInsights.properties.InstrumentationKey
            }
            {
              name: 'GameServiceUrl'
              value: 'http://${gameServiceFqdn}'
            }
          ]
          resources: {
            cpu: json(cpuCore)
            memory: '${memorySize}Gi'
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
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
            name: 'http-trigger'
            http: {
              metadata: {
                concurrentRequests: scaleConcurrentRequests
              }
            }
          }
        ]
      }
    }
  }
}
