param location string = resourceGroup().location
param serviceBusName string
param skuName string = 'Standard'

resource serviceBus 'Microsoft.ServiceBus/namespaces@2017-04-01' = {
  name: serviceBusName
  location: location
  sku: {
    name: skuName
  }
}
