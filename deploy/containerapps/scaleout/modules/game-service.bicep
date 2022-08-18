param location string = resourceGroup().location
param containerAppName string
param environmentId string
param containerRegistry string
param containerImage string
param appInsightsName string
param signalRName string
param cpuCore string
param memorySize string
param minReplicas int
param maxReplicas int
param scaleConcurrentRequests string

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource signalR 'Microsoft.SignalRService/signalR@2021-10-01' existing = {
  name: signalRName
}

var signalRConnectionString = signalR.listKeys().primaryConnectionString

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      ingress: {
        external: false
        targetPort: 80
        allowInsecure: true
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
      secrets: [
        {
          name: 'signalr-connection'
          value: signalRConnectionString
        }
      ]
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
              name: 'AzureSignalRConnectionString'
              secretRef: 'signalr-connection'
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

output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn
