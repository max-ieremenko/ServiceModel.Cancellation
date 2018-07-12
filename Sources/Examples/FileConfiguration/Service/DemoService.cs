using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using ServiceModel.Cancellation;

namespace Service
{
    [ServiceBehavior(ConfigurationName = "ServiceConfigurationName")]
    public sealed class DemoService : IDemoService
    {
        public async Task<OperationResult> RunOperationAsync(TimeSpan delay, CancellationTokenProxy token)
        {
            var timer = Stopwatch.StartNew();

            try
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
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
}