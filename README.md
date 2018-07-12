# ServiceModel.Cancellation library for .Net 
CancellationToken support for WCF.

#### Supported platforms
[.NET Framework 4.5.2](https://www.microsoft.com/en-us/download/details.aspx?id=42642) or higher.

## Service-side
```C#
using CancellationTokenProxy = ServiceModel.Cancellation.CancellationTokenProxy;

[ServiceContract]
[UseCancellation] // enable cancellation support for service
public class DemoService
{
    // accept request with CancellationSourceToken
    [OperationContract]
    public async Task<OperationResult> RunOperationAsync(TimeSpan delay, CancellationTokenProxy token)
    {
        var timer = Stopwatch.StartNew();

        try
        {
            await Task.Delay(delay, token);
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken == token)
        {
        }

        return new OperationResult
        {
            ExecutionTime = timer.Elapsed,
            IsCanceled = token.IsCancellationRequested
        };
    }
}
```

## Client-side
```C#
using (var cancellationSource = new CancellationTokenSource())
using (var clientFactory = new ChannelFactory<IDemoService>())
{
    // enable cancellation support for client
    clientFactory.UseCancellation();

    // cancel request after 1 second
    cancellationSource.CancelAfter(TimeSpan.FromSeconds(1));

    var client = clientFactory.CreateChannel();

    // pass CancellationSourceToken to service
    var response = await client.RunOperationAsync(
        TimeSpan.FromSeconds(5),
        cancellationSource.Token);

    Console.WriteLine("ExecutionTime: {0}", response.ExecutionTime);
    Console.WriteLine("IsCanceled: {0}", response.IsCanceled);
}
```

## Todo list to setup your code and environment
#### Service-side
- each service has to be pre-configured to support cancellation
- your environment has to host entry point ([see CancellationContractService](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/ServiceModel.Cancellation/Service/CancellationContractService.cs))) to accept cancellation requests from client

#### Client-side
- each client channel has to be pre-configured to support cancellation
- to pass cancellation requests to service [CancellationContractClient](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/ServiceModel.Cancellation/Client/CancellationContractClient.cs) will be used by default

## Examples
- [CodeConfiguration](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/CodeConfiguration) demonstrates how to configure ServiceModel.Cancellation from code
- [FileConfiguration](https://github.com/max-ieremenko/ServiceModel.Cancellation/blob/master/Sources/Examples/FileConfiguration) demonstrates how to configure ServiceModel.Cancellation from application configuration file.

## License
This tool is distributed under the [MIT](https://github.com/max-ieremenko/ServiceModel.Cancellation/tree/master/LICENSE) license.
