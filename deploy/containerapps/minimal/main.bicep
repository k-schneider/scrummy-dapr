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

targetScope = 'subscription'

var logAnalyticsWorkspaceName = '${resourceBaseName}-logs'
var appInsightsName = '${resourceBaseName}-appinsights'
var containerAppEnvName = '${resourceBaseName}-env'
var containerAppName = '${resourceBaseName}-app'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${resourceBaseName}-rg'
  location: location
}

module environmentDeploy 'modules/environment.bicep' = {
  name: 'environmentDeploy'
  scope: rg
  params: {
    location: location
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    appInsightsName: appInsightsName
    containerAppEnvName: containerAppEnvName
    containerAppName: containerAppName
  }
}

module containerAppDeploy 'modules/container-app.bicep' = {
  name: 'containerAppDeploy'
  scope: rg
  params: {
    location: location
    containerAppName: containerAppName
    environmentId: environmentDeploy.outputs.environmentId
    appInsightsName: appInsightsName
  }
}
