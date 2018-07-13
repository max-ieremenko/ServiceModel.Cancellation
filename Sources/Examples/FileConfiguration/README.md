# File configuration example
Demonstrates how to configure ServiceModel.Cancellation from application configuration file.

## Service
[DemoService](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/FileConfiguration/Service/DemoService.cs) is configured to support cancellation in [Web.config](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/FileConfiguration/Service/Web.config):
```C#
    [ServiceBehavior(ConfigurationName = "ServiceConfigurationName")]
    public sealed class DemoService : IDemoService
    {
```
```xml
<configuration>
  <system.serviceModel>
    <services>
      <service name="ServiceConfigurationName">
        <!-- !!! ServiceModel.Cancellation demo: set-up service endpoint to use behavior with cancellation support -->
        <endpoint binding="basicHttpBinding"
                  behaviorConfiguration="CancellationBehaviorConfiguration"
                  contract="Service.IDemoService"/>
      </service>
    </services>

    <behaviors>
      <endpointBehaviors>
        <!-- !!! ServiceModel.Cancellation demo: define behavior with cancellation support -->
        <behavior name="CancellationBehaviorConfiguration">
          <useCancellation />
        </behavior>
      </endpointBehaviors>
    </behaviors>

    <extensions>
      <behaviorExtensions>
        <!-- !!! ServiceModel.Cancellation demo: register cancellation behavior extension -->
        <add name="useCancellation"
             type="ServiceModel.Cancellation.CancellationBehaviorElement, ServiceModel.Cancellation"/>
      </behaviorExtensions>
    </extensions>
```

[Web.config](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/FileConfiguration/Service/Web.config) defines entry point ~/CancellationContractService.svc to accept cancellation requests from client:
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
Auto-generated [DemoServiceClient](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/FileConfiguration/Client/Connected%20Services/Generated/Reference.cs) is configured to support cancellation in [App.config](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/FileConfiguration/Client/App.config):
```xml
<configuration>
  <system.serviceModel>
    <client>
      <!-- !!! ServiceModel.Cancellation demo: define behavior with cancellation support -->
      <endpoint name="BasicHttpBinding_IDemoService"
                address="http://localhost:51599/DemoService.svc"
                binding="basicHttpBinding"
                bindingConfiguration="BasicHttpBinding_IDemoService"
                behaviorConfiguration="CancellationBehaviorWithDefaultOptions"
                contract="Generated.IDemoService" />
    </client>

    <behaviors>
      <endpointBehaviors>
        <!-- !!! ServiceModel.Cancellation demo: define behavior with cancellation support -->
        <behavior name="CancellationBehaviorWithDefaultOptions">
          <useCancellation />
        </behavior>
      </endpointBehaviors>
    </behaviors>

    <extensions>
      <behaviorExtensions>
        <!-- !!! ServiceModel.Cancellation demo: register cancellation behavior extension -->
        <add name="useCancellation"
             type="ServiceModel.Cancellation.CancellationBehaviorElement, ServiceModel.Cancellation"/>
      </behaviorExtensions>
    </extensions>
  </system.serviceModel>
</configuration>
```

Notifications from CancellationToken will be propagated via [CancellationContractClient](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/ServiceModel.Cancellation/Client/CancellationContractClient.cs), configured in [App.config](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/FileConfiguration/Client/App.config):
```xml
<configuration>
    <client>
      <!-- !!! ServiceModel.Cancellation demo: register channel to pass cancellation requests -->
      <endpoint address="http://localhost:51599/CancellationContractService.svc"
                binding="basicHttpBinding"
                contract="ServiceModel.Cancellation.ICancellationContract" />
    </client>
  </system.serviceModel>
</configuration>
```

[CancellationContractClientFactory](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/FileConfiguration/Client/CancellationContractClientFactory.cs) allows to control how to instantiate and configure CancellationContractClient:
```xml
<configuration>
  <system.serviceModel>
    <behaviors>
      <endpointBehaviors>
        <!-- !!! ServiceModel.Cancellation demo: define behavior with cancellation support and custom client factory -->
        <behavior name="CancellationBehaviorWithCustomizedOptions">
          <useCancellation>
            <clientOptions contractFactory="Client.CancellationContractClientFactory.CreateClient, Client" />
          </useCancellation>
        </behavior>
      </endpointBehaviors>
    </behaviors>
  </system.serviceModel>
</configuration>
```
