param location string = resourceGroup().location
param redisName string

resource redis 'Microsoft.Cache/Redis@2021-06-01' = {
  name: redisName
  location: location
  properties: {
    redisVersion: '6.0.14'
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
    enableNonSslPort: true
    publicNetworkAccess: 'Enabled'
    redisConfiguration: {
      'maxmemory-reserved': '30'
      'maxfragmentationmemory-reserved': '30'
      'maxmemory-delta': '30'
    }
  }
}

output name string = redis.name
