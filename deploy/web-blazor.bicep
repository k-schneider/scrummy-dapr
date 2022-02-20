param location string = resourceGroup().location
param resourceBaseName string
param environmentId string
param containerRegistry string
param containerRegistryPrivate bool
param containerRegistryUsername string
@secure()
param containerRegistryPassword string
param containerImage string
param gameServiceFqdn string
param logAnalyticsWorkspaceName string
param appInsightsName string
param cpuCore string
param memorySize string
param minReplicas int
param maxReplicas int

var containerAppName = '${resourceBaseName}-web-blazor'
var containerPort = 80

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' existing = {
  name: logAnalyticsWorkspaceName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

var analyticsWorkspaceId = logAnalyticsWorkspace.properties.customerId
var analyticsPrimaryKey = listKeys(logAnalyticsWorkspace.id, logAnalyticsWorkspace.apiVersion).primarySharedKey

resource containerApp 'Microsoft.Web/containerApps@2021-03-01' = {
  name: containerAppName
  kind: 'containerapp'
  location: location
  properties: {
    kubeEnvironmentId: environmentId
    configuration: {
      ingress: {
        external: true
        targetPort: containerPort
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
          name: 'registry-password'
          value: containerRegistryPassword
        }
        {
          name: 'analytics-key'
          value: analyticsPrimaryKey
        }
      ]
      registries: containerRegistryPrivate ? [
        {
          server: containerRegistry
          username: containerRegistryUsername
          passwordSecretRef: 'registry-password'
        }
      ] : []
    }
    template: {
      containers: [
        {
          image: '${containerRegistry}/${containerImage}'
          name: containerAppName
          env: [
            {
              name: 'AzureAnalyticsWorkspaceId'
              value: analyticsWorkspaceId
            }
            {
              name: 'AzureAnalyticsPrimaryKey'
              secretRef: 'analytics-key'
            }
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

output containerAppName string = containerApp.name
