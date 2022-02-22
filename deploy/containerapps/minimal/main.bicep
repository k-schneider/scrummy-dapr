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

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${resourceBaseName}-rg'
  location: location
}

module environmentDeploy 'modules/environment.bicep' = {
  name: 'environmentDeploy'
  scope: rg
  params: {
    location: location
    resourceBaseName: resourceBaseName
  }
}

module containerAppDeploy 'modules/container-app.bicep' = {
  name: 'containerAppDeploy'
  scope: rg
  params: {
    location: location
    resourceBaseName: resourceBaseName
    environmentId: environmentDeploy.outputs.environmentId
    appInsightsName: environmentDeploy.outputs.appInsightsName
  }
}
