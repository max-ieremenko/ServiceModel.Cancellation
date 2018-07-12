using System.Threading;

namespace ServiceModel.Cancellation.Service
{
    internal interface IServiceTokenManager
    {
        CancellationToken BeginOperation(string operationId);

        void EndOperation(string operationId);

        void CancelOperation(string operationId);
    }
}
