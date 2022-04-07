param location string = resourceGroup().location
param logAnalyticsWorkspaceName string
param appInsightsName string
param containerAppEnvName string
param cosmosDbAccountName string
param cosmosDbDatabaseName string
param cosmosDbContainerName string
param serviceBusName string
param gameServiceContainerAppName string

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' existing = {
  name: cosmosDbAccountName
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2017-04-01' existing = {
  name: serviceBusName
}

var cosmosDbMasterKey = databaseAccount.listKeys().primaryMasterKey
var cosmosDbEndpoint = databaseAccount.properties.documentEndpoint

var serviceBusEndpoint = '${serviceBus.id}/AuthorizationRules/RootManageSharedAccessKey'
var serviceBusConnectionString = listKeys(serviceBusEndpoint, serviceBus.apiVersion).primaryConnectionString

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
    IngestionMode: 'LogAnalytics'
    RetentionInDays: 30
  }
}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2022-01-01-preview' = {
  name: containerAppEnvName
  location: location
  properties: {
    daprAIInstrumentationKey: reference(appInsights.id, '2020-02-02').InstrumentationKey
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: reference(logAnalyticsWorkspace.id, '2020-03-01-preview').customerId
        sharedKey: listKeys(logAnalyticsWorkspace.id, '2020-03-01-preview').primarySharedKey
      }
    }
  }
  resource redisStateComponent 'daprComponents@2022-01-01-preview' = {
    name: 'statestore'
    properties: {
      componentType: 'state.azure.cosmosdb'
      version: 'v1'
      ignoreErrors: false
      initTimeout: '5m'
      secrets: [
        {
          name: 'cosmos-key'
          value: cosmosDbMasterKey
        }
      ]
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
      scopes: [
        gameServiceContainerAppName
      ]
    }
  }
  resource redisPubSubComponent 'daprComponents@2022-01-01-preview' = {
    name: 'pubsub'
    properties: {
      componentType: 'pubsub.azure.servicebus'
      version: 'v1'
      ignoreErrors: false
      initTimeout: '5s'
      secrets: [
        {
          name: 'sb-connection'
          value: serviceBusConnectionString
        }
      ]
      metadata: [
        {
          name: 'connectionString'
          secretRef: 'sb-connection'
        }
      ]
      scopes: [
        gameServiceContainerAppName
      ]
    }
  }
}

output environmentId string = containerAppEnv.id
