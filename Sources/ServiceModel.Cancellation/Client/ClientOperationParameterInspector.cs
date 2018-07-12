using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Client
{
    internal sealed class ClientOperationParameterInspector : IParameterInspector
    {
        public ClientOperationParameterInspector(
            IClientOperationManager operationManager,
            IDictionary<string, CancellableOperationDescription> operationByName)
        {
            operationManager.IsNotNull(nameof(operationManager));
            operationByName.IsNotNullAndNotEmpty(nameof(operationByName));

            OperationManager = operationManager;
            OperationByName = operationByName;
        }

        public IClientOperationManager OperationManager { get; }

        public IDictionary<string, CancellableOperationDescription> OperationByName { get; }

        public object BeforeCall(string operationName, object[] inputs)
        {
            if (!OperationByName.TryGetValue(operationName, out var description))
            {
                // method does not contain token
                return null;
            }

            var token = description.GetToken(inputs);
            var operation = OperationManager.BeforeCall(description.FullName, token);

            if (operation != null)
            {
                description.PassTokenIntoChannel(inputs, operation.Token);
            }

            return operation;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            var operation = (OperationInfo)correlationState;
            if (operation != null)
            {
                OperationManager.AfterCall(operation);
            }
        }
    }
}