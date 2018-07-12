using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Service
{
    internal sealed class DispatchOperationManager : IDispatchOperationManager
    {
        public DispatchOperationManager(IServiceTokenManager manager)
        {
            manager.IsNotNull(nameof(manager));

            Manager = manager;
        }

        public IServiceTokenManager Manager { get; }

        public OperationInfo BeforeCall(string operationName, CancellationTokenProxy? token)
        {
            operationName.IsNotNullAndNotEmpty(nameof(operationName));

            var operationId = token?.OperationId;
            if (string.IsNullOrEmpty(operationId))
            {
                return null;
            }

            var operationToken = Manager.BeginOperation(operationId);

            return new OperationInfo
            {
                Token = token.Value.NewToken(operationToken)
            };
        }

        public void AfterCall(OperationInfo operation)
        {
            operation.IsNotNull(nameof(operation));

            Manager.EndOperation(operation.Token.OperationId);
        }
    }
}
