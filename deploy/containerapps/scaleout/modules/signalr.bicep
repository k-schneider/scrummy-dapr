param location string = resourceGroup().location
param signalrName string
param skuName string = 'Standard_S1'
param capacity int = 1

resource signalR 'Microsoft.SignalRService/signalR@2021-10-01' = {
  name: signalrName
  location: location
  sku: {
    name: skuName
    capacity: capacity
  }
  properties: {
    features: [
      {
        flag: 'ServiceMode'
        value: 'Default'
      }
    ]
  }
}
