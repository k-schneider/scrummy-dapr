param location string = resourceGroup().location
param resourceBaseName string
param skuName string = 'Standard'

resource serviceBus 'Microsoft.ServiceBus/namespaces@2017-04-01' = {
  name: resourceBaseName
  location: location
  sku: {
    name: skuName
  }
}

output serviceBusName string = serviceBus.name
