param location string = resourceGroup().location
param logAnalyticsWorkspaceName string
param appInsightsName string
param containerAppEnvName string
param containerAppName string

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
      componentType: 'state.redis'
      version: 'v1'
      ignoreErrors: false
      initTimeout: '5s'
      secrets: [
        {
          name: 'redis-password'
          value: 'password'
        }
      ]
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
      scopes: [
        containerAppName
      ]
    }
  }
  resource redisPubSubComponent 'daprComponents@2022-01-01-preview' = {
    name: 'pubsub'
    properties: {
      componentType: 'pubsub.redis'
      version: 'v1'
      ignoreErrors: false
      initTimeout: '5s'
      secrets: [
        {
          name: 'redis-password'
          value: 'password'
        }
      ]
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
      scopes: [
        containerAppName
      ]
    }
  }
}

output environmentId string = containerAppEnv.id
