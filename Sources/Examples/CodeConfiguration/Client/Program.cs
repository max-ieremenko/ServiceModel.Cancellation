using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Client.Generated;
using ServiceModel.Cancellation;

namespace Client
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Code configuration example");

            try
            {
                await InvokeDemoService().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("...");
                Console.ReadLine();
            }
        }

        private static async Task InvokeDemoService()
        {
            OperationResult response;

            using (var cancellationSource = new CancellationTokenSource())
            using (var client = new DemoServiceClient())
            {
                // !!! ServiceModel.Cancellation demo: set-up client to support cancellation
                client.UseCancellation();

                ////// !!! ServiceModel.Cancellation demo: set-up client to support cancellation custom client factory
                ////client.UseCancellation(o => o.ContractFactory = CancellationContractClientFactory.CreateClient);

                cancellationSource.CancelAfter(TimeSpan.FromSeconds(1));

                response = await client.RunOperationAsync(TimeSpan.FromSeconds(5), cancellationSource.Token).ConfigureAwait(false);
            }

            Console.WriteLine("ExecutionTime: {0}", response.ExecutionTime);
            Console.WriteLine("IsCanceled: {0}", response.IsCanceled);
        }
    }
}
