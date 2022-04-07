@description('Specifies the location for all resources.')
@allowed([
  'eastus'
  'northcentralusstage'
  'northeurope'
  'canadacentral'
])
param location string

@description('Base name used when constructing all resources.')
param resourceBaseName string

@description('The throughput policy for Cosmos DB')
@allowed([
  'Manual'
  'Autoscale'
])
param cosmosThroughputPolicy string = 'Manual'

@description('Throughput value when using Manual Throughput Policy for Cosmos DB')
@minValue(400)
@maxValue(1000000)
param cosmosManualProvisionedThroughput int = 400

@description('Maximum throughput when using Autoscale Throughput Policy for Cosmos DB')
@minValue(4000)
@maxValue(1000000)
param cosmosAutoscaleMaxThroughput int = 4000

@description('Enable free tier for Cosmos DB.')
param cosmosEnableFreeTier bool = false

@description('Container registry to pull images from.')
param containerRegistry string

@description('Game service container image.')
param gameServiceContainerImage string

@description('Game service min replicas.')
@minValue(0)
@maxValue(10)
param gameServiceMinReplicas int = 0

@description('Game service max replicas.')
@minValue(1)
@maxValue(10)
param gameServiceMaxReplicas int = 10

@description('Game service required CPU in cores.')
param gameServiceCpuCore string = '0.5'

@description('Game service required memory size in gigabytes.')
param gameServiceMemorySize string = '1'

@description('Web blazor container image.')
param webBlazorContainerImage string

@description('Web blazor min replicas.')
@minValue(0)
@maxValue(10)
param webBlazorMinReplicas int = 0

@description('Web blazor max replicas.')
@minValue(1)
@maxValue(10)
param webBlazorMaxReplicas int = 10

@description('Web blazor required CPU in cores.')
param webBlazorCpuCore string = '0.5'

@description('Web blazor required memory size in gigabytes.')
param webBlazorMemorySize string = '1'

targetScope = 'subscription'

var cosmosDbAccountName = '${resourceBaseName}-cosmos'
var cosmosDbDatabaseName = resourceBaseName
var cosmosDbContainerName = 'dapr'
var serviceBusName = resourceBaseName
var signalrName = resourceBaseName
var logAnalyticsWorkspaceName = '${resourceBaseName}-logs'
var appInsightsName = '${resourceBaseName}-appinsights'
var containerAppEnvName = '${resourceBaseName}-env'
var gameServiceContainerAppName = '${resourceBaseName}-game-service'
var webBlazorContainerAppName = '${resourceBaseName}-web-blazor'

// If game service has more than one replica then we need Azure SignalR
var createAzureSignalR = gameServiceMaxReplicas > 1

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${resourceBaseName}-rg'
  location: location
}

module cosmosDbDeploy 'modules/cosmos-db.bicep' = {
  name: 'cosmosDbDeploy'
  scope: rg
  params: {
    location: location
    databaseAccountName: cosmosDbAccountName
    databaseName: cosmosDbDatabaseName
    databaseContainerName: cosmosDbContainerName
    throughputPolicy: cosmosThroughputPolicy
    manualProvisionedThroughput: cosmosManualProvisionedThroughput
    autoscaleMaxThroughput: cosmosAutoscaleMaxThroughput
    enableFreeTier: cosmosEnableFreeTier
  }
}

module serviceBusDeploy 'modules/service-bus.bicep' = {
  name: 'serviceBusDeploy'
  scope: rg
  params: {
    location: location
    serviceBusName: serviceBusName
  }
}

module signalRDeploy 'modules/signalr.bicep' = if (createAzureSignalR) {
  name: 'signalRDeploy'
  scope: rg
  params: {
    location: location
    signalrName: signalrName
  }
}

module environmentDeploy 'modules/environment.bicep' = {
  name: 'environmentDeploy'
  scope: rg
  params: {
    location: location
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    appInsightsName: appInsightsName
    containerAppEnvName: containerAppEnvName
    cosmosDbAccountName: cosmosDbAccountName
    cosmosDbDatabaseName: cosmosDbDatabaseName
    cosmosDbContainerName: cosmosDbContainerName
    serviceBusName: serviceBusName
    gameServiceContainerAppName: gameServiceContainerAppName
  }
}

module gameServiceDeploy 'modules/game-service.bicep' = {
  name: 'gameServiceDeploy'
  scope: rg
  params: {
    location: location
    containerAppName: gameServiceContainerAppName
    environmentId: environmentDeploy.outputs.environmentId
    containerRegistry: containerRegistry
    containerImage: gameServiceContainerImage
    appInsightsName: appInsightsName
    signalRName: createAzureSignalR ? signalrName : ''
    minReplicas: gameServiceMinReplicas
    maxReplicas: gameServiceMaxReplicas
    cpuCore: gameServiceCpuCore
    memorySize: gameServiceMemorySize
  }
}

module webBlazorDeploy 'modules/web-blazor.bicep' = {
  name: 'webBlazorDeploy'
  scope: rg
  params: {
    location: location
    containerAppName: webBlazorContainerAppName
    environmentId: environmentDeploy.outputs.environmentId
    containerRegistry: containerRegistry
    containerImage: webBlazorContainerImage
    gameServiceFqdn: gameServiceDeploy.outputs.containerAppFqdn
    appInsightsName: appInsightsName
    minReplicas: webBlazorMinReplicas
    maxReplicas: webBlazorMaxReplicas
    cpuCore: webBlazorCpuCore
    memorySize: webBlazorMemorySize
  }
}
