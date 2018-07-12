using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Client.Generated;

namespace Client
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("File configuration example");

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
                cancellationSource.CancelAfter(TimeSpan.FromSeconds(1));

                response = await client.RunOperationAsync(TimeSpan.FromSeconds(5), cancellationSource.Token).ConfigureAwait(false);
            }

            Console.WriteLine("ExecutionTime: {0}", response.ExecutionTime);
            Console.WriteLine("IsCanceled: {0}", response.IsCanceled);
        }
    }
}
