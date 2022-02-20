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

@description('Is the container registry private?')
param containerRegistryPrivate bool = false

@description('Username to log into container registry.')
param containerRegistryUsername string = ''

@description('Password to log into container registry.')
@secure()
param containerRegistryPassword string = ''

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

// If game service has more than one replica then we need Azure SignalR
var createAzureSignalR = gameServiceMaxReplicas > 1

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${resourceBaseName}-rg'
  location: location
}

module environmentDeploy 'environment.bicep' = {
  name: 'environmentDeploy'
  scope: rg
  params: {
    location: location
    resourceBaseName: resourceBaseName
  }
}

module cosmosDbDeploy 'cosmos-db.bicep' = {
  name: 'cosmosDbDeploy'
  scope: rg
  params: {
    location: location
    resourceBaseName: resourceBaseName
    throughputPolicy: cosmosThroughputPolicy
    manualProvisionedThroughput: cosmosManualProvisionedThroughput
    autoscaleMaxThroughput: cosmosAutoscaleMaxThroughput
    enableFreeTier: cosmosEnableFreeTier
  }
}

module serviceBusDeploy 'service-bus.bicep' = {
  name: 'serviceBusDeploy'
  scope: rg
  params: {
    location: location
    resourceBaseName: resourceBaseName
  }
}

module signalRDeploy 'signalr.bicep' = if (createAzureSignalR) {
  name: 'signalRDeploy'
  scope: rg
  params: {
    location: location
    resourceBaseName: resourceBaseName
  }
}

module gameServiceDeploy 'game-service.bicep' = {
  name: 'gameServiceDeploy'
  scope: rg
  params: {
    location: location
    resourceBaseName: resourceBaseName
    environmentId: environmentDeploy.outputs.environmentId
    containerRegistry: containerRegistry
    containerRegistryPrivate: containerRegistryPrivate
    containerRegistryUsername: containerRegistryUsername
    containerRegistryPassword: containerRegistryPassword
    containerImage: gameServiceContainerImage
    cosmosDbAccountName: cosmosDbDeploy.outputs.accountName
    cosmosDbDatabaseName: cosmosDbDeploy.outputs.databaseName
    cosmosDbContainerName: cosmosDbDeploy.outputs.containerName
    logAnalyticsWorkspaceName: environmentDeploy.outputs.logAnalyticsWorkspaceName
    appInsightsName: environmentDeploy.outputs.appInsightsName
    serviceBusName: serviceBusDeploy.outputs.serviceBusName
    signalRName: createAzureSignalR ? signalRDeploy.outputs.signalRName : ''
    minReplicas: gameServiceMinReplicas
    maxReplicas: gameServiceMaxReplicas
    cpuCore: gameServiceCpuCore
    memorySize: gameServiceMemorySize
  }
}

module webBlazorDeploy 'web-blazor.bicep' = {
  name: 'webBlazorDeploy'
  scope: rg
  params: {
    location: location
    resourceBaseName: resourceBaseName
    environmentId: environmentDeploy.outputs.environmentId
    containerRegistry: containerRegistry
    containerRegistryPrivate: containerRegistryPrivate
    containerRegistryUsername: containerRegistryUsername
    containerRegistryPassword: containerRegistryPassword
    containerImage: webBlazorContainerImage
    gameServiceFqdn: gameServiceDeploy.outputs.containerAppFqdn
    logAnalyticsWorkspaceName: environmentDeploy.outputs.logAnalyticsWorkspaceName
    appInsightsName: environmentDeploy.outputs.appInsightsName
    minReplicas: webBlazorMinReplicas
    maxReplicas: webBlazorMaxReplicas
    cpuCore: webBlazorCpuCore
    memorySize: webBlazorMemorySize
  }
}
