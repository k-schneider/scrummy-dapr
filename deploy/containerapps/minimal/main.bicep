@description('Specifies the location for all resources.')
param location string = resourceGroup().location

@description('Base name used when constructing all resources.')
param resourceBaseName string

var logAnalyticsWorkspaceName = '${resourceBaseName}-logs'
var appInsightsName = '${resourceBaseName}-appinsights'
var containerAppEnvName = '${resourceBaseName}-env'
var containerAppName = '${resourceBaseName}-app'

module environmentDeploy 'modules/environment.bicep' = {
  name: 'environmentDeploy'
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
  params: {
    location: location
    containerAppName: containerAppName
    environmentId: environmentDeploy.outputs.environmentId
    appInsightsName: appInsightsName
  }
}
