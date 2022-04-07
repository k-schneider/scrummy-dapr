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

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource signalR 'Microsoft.SignalRService/signalR@2021-10-01' existing = if(signalRName != '') {
  name: signalRName
}

var signalRConnectionString = signalRName == '' ? '' : signalR.listKeys().primaryConnectionString

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
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
            cpu: cpuCore
            memory: '${memorySize}Gi'
          }
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn
