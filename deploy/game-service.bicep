param location string = resourceGroup().location
param resourceBaseName string
param environmentId string
param containerRegistry string
param containerRegistryPrivate bool
param containerRegistryUsername string
@secure()
param containerRegistryPassword string
param containerImage string
param cosmosDbAccountName string
param cosmosDbDatabaseName string
param cosmosDbContainerName string
param logAnalyticsWorkspaceName string
param appInsightsName string
param serviceBusName string
param signalRName string
param cpuCore string
param memorySize string
param minReplicas int
param maxReplicas int

var containerAppName = '${resourceBaseName}-game-service'
var containerPort = 80

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' existing = {
  name: cosmosDbAccountName
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' existing = {
  name: logAnalyticsWorkspaceName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2017-04-01' existing = {
  name: serviceBusName
}

resource signalR 'Microsoft.SignalRService/signalR@2021-10-01' existing = if(signalRName != '') {
  name: signalRName
}

var analyticsWorkspaceId = logAnalyticsWorkspace.properties.customerId
var analyticsPrimaryKey = listKeys(logAnalyticsWorkspace.id, logAnalyticsWorkspace.apiVersion).primarySharedKey

var cosmosDbMasterKey = databaseAccount.listKeys().primaryMasterKey
var cosmosDbEndpoint = databaseAccount.properties.documentEndpoint

var serviceBusEndpoint = '${serviceBus.id}/AuthorizationRules/RootManageSharedAccessKey'
var serviceBusConnectionString = listKeys(serviceBusEndpoint, serviceBus.apiVersion).primaryConnectionString

var signalRConnectionString = signalRName == '' ? '' : signalR.listKeys().primaryConnectionString

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
          name: 'registry-password'
          value: containerRegistryPassword
        }
        {
          name: 'cosmos-key'
          value: cosmosDbMasterKey
        }
        {
          name: 'analytics-key'
          value: analyticsPrimaryKey
        }
        {
          name: 'sb-connection'
          value: serviceBusConnectionString
        }
        {
          name: 'signalr-connection'
          value: signalRConnectionString
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
              name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
              value: appInsights.properties.InstrumentationKey
            }
            {
              name: 'AzureAnalyticsWorkspaceId'
              value: analyticsWorkspaceId
            }
            {
              name: 'AzureAnalyticsPrimaryKey'
              secretRef: 'analytics-key'
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
      dapr: {
        enabled: true
        appPort: containerPort
        appId: '${containerAppName}-dapr'
        components: [
          {
            name: 'statestore'
            type: 'state.azure.cosmosdb'
            version: 'v1'
            initTimeout: '5m'
            metadata: [
              {
                name: 'url'
                value: cosmosDbEndpoint
              }
              {
                name: 'masterKey'
                secretRef: 'cosmos-key'
              }
              {
                name: 'database'
                value: cosmosDbDatabaseName
              }
              {
                name: 'collection'
                value: cosmosDbContainerName
              }
              {
                name: 'actorStateStore'
                value: 'true'
              }
            ]
          }
          {
            name: 'pubsub'
            type: 'pubsub.azure.servicebus'
            version: 'v1'
            metadata: [
              {
                name: 'connectionString'
                secretRef: 'sb-connection'
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
