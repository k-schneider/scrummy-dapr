param location string = resourceGroup().location
param resourceBaseName string
param throughputPolicy string
param manualProvisionedThroughput int
param autoscaleMaxThroughput int
param enableFreeTier bool

var locations = [
  {
    locationName: location
    failoverPriority: 0
    isZoneRedundant: false
  }
]

var throughputPolicies = {
  Manual: {
    Throughput: manualProvisionedThroughput
  }
  Autoscale: {
    autoscaleSettings: {
      maxThroughput: autoscaleMaxThroughput
    }
  }
}

var containerName = 'dapr'

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: '${resourceBaseName}-cosmos'
  location: location
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
    locations: locations
    enableFreeTier: enableFreeTier
    /*
    enableAnalyticalStorage: true
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    */
  }
}

resource sqlDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-04-15' = {
  parent: databaseAccount
  name: resourceBaseName
  properties: {
    resource: {
      id: resourceBaseName
    }
  }
}

resource sqlContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-04-15' = {
  parent: sqlDatabase
  name: containerName
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          '/partitionKey'
        ]
        kind: 'Hash'
      }
    }
    options: throughputPolicies[throughputPolicy]
  }
}

output accountName string = databaseAccount.name
output databaseName string = sqlDatabase.name
output containerName string = sqlContainer.name
