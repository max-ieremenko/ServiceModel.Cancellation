using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Client
{
    internal sealed class ClientOperationManager : IClientOperationManager
    {
        public ClientOperationManager(Func<ICancellationContract> contractFactory)
        {
            contractFactory.IsNotNull(nameof(contractFactory));

            ContractFactory = contractFactory;
        }

        public Func<ICancellationContract> ContractFactory { get; }

        public OperationInfo BeforeCall(string operationName, CancellationTokenProxy? token)
        {
            operationName.IsNotNullAndNotEmpty(nameof(operationName));

            var tokenValue = token.GetValueOrDefault();
            if (!tokenValue.CanBeCanceled)
            {
                // something is there, but does not matter
                return null;
            }

            var operation = new OperationInfo();
            operation.Token = tokenValue.NewOperationId(GenerateOperationId(operationName));
            operation.TokenRegistration = tokenValue.Register(i => OnTokenCanceled((OperationInfo)i), operation, false);

            return operation;
        }

        public void AfterCall(OperationInfo operation)
        {
            operation.IsNotNull(nameof(operation));

            operation.TokenRegistration?.Dispose();
        }

        internal static string GenerateOperationId(string operationFullName)
        {
            // IService.Method?20180607151640123.guid
            return "{0}?{1}.{2}".FormatWith(
                operationFullName,
                DateTime.UtcNow.ToString("yyMMddHHmmssfff", CultureInfo.InvariantCulture),
                Guid.NewGuid().ToString("N"));
        }

        private void OnTokenCanceled(OperationInfo operation)
        {
            AfterCall(operation);

            var proxy = ContractFactory();
            operation.Cancellation = proxy
                .CancelAsync(operation.Token.OperationId)
                .ContinueWith(
                    t =>
                    {
                        (proxy as IDisposable)?.Dispose();
                        if (t.IsFaulted)
                        {
                            Trace.WriteLine(t.Exception);
                        }
                    },
                    TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
