param location string = resourceGroup().location
param resourceBaseName string
param skuName string = 'Standard_S1'
param capacity int = 1

resource signalR 'Microsoft.SignalRService/signalR@2021-10-01' = {
  name: resourceBaseName
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

output signalRName string = signalR.name
