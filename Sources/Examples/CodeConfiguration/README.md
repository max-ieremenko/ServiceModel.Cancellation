# Code configuration example
Demonstrates how to configure ServiceModel.Cancellation from code.

## Service
Service host factory [DemoServiceFactory](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/CodeConfiguration/Service/DemoServiceFactory.cs) enables cancellation support for [DemoService](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/CodeConfiguration/Service/DemoService.cs):
```C#
    var host = new ServiceHost(typeof(DemoService), baseAddresses);

    // !!! ServiceModel.Cancellation demo: set-up host to support cancellation
    host.UseCancellation();
```

[Web.config](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/CodeConfiguration/Service/Web.config) defines entry point ~/CancellationContractService.svc to accept cancellation requests from client:
```xml
<configuration>
  <system.serviceModel>
    <serviceHostingEnvironment>
      <serviceActivations>
        <!-- !!! ServiceModel.Cancellation demo: register cancellation entry point -->
        <add relativeAddress="~/CancellationContractService.svc"
             service="ServiceModel.Cancellation.Service.CancellationContractService, ServiceModel.Cancellation" />

      </serviceActivations>
    </serviceHostingEnvironment>
  </system.serviceModel>
</configuration>
```

## Client
Reference to Service is auto-generated via "Add Service Reference...". 
Following code enables cancellation support for auto-generated [DemoServiceClient](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/CodeConfiguration/Client/Connected%20Services/Generated/Reference.cs):
```C#
    using (var client = new DemoServiceClient())
    {
        // !!! ServiceModel.Cancellation demo: set-up client to support cancellation
        client.UseCancellation();
```

Notifications from CancellationToken will be propagated via [CancellationContractClient](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/ServiceModel.Cancellation/Client/CancellationContractClient.cs), configured in [App.config](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/CodeConfiguration/Client/App.config):
```xml
<configuration>
    <client>
      <!-- !!! ServiceModel.Cancellation demo: register channel to pass cancellation requests -->
      <endpoint address="http://localhost:52003/CancellationContractService.svc"
                binding="basicHttpBinding"
                contract="ServiceModel.Cancellation.ICancellationContract" />
    </client>
  </system.serviceModel>
</configuration>
```

[CancellationContractClientFactory](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/CodeConfiguration/Client/CancellationContractClientFactory.cs) allows to control how to instantiate and configure CancellationContractClient:
```C#
    using (var client = new DemoServiceClient())
    {
        // !!! ServiceModel.Cancellation demo: set-up client to support cancellation custom client factory
        client.UseCancellation(o => o.ContractFactory = CancellationContractClientFactory.CreateClient);
```
