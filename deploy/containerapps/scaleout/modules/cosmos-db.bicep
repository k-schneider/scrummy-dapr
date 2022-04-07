param location string = resourceGroup().location
param databaseAccountName string
param databaseName string
param databaseContainerName string
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

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: databaseAccountName
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
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource sqlContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-04-15' = {
  parent: sqlDatabase
  name: databaseContainerName
  properties: {
    resource: {
      id: databaseContainerName
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
