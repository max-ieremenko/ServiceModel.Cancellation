using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Service
{
    internal sealed class DispatchOperationParameterInspector : IParameterInspector
    {
        public DispatchOperationParameterInspector(
            IDispatchOperationManager operationManager,
            IDictionary<string, CancellableOperationDescription> operationByName)
        {
            operationManager.IsNotNull(nameof(operationManager));
            operationByName.IsNotNullAndNotEmpty(nameof(operationByName));

            OperationManager = operationManager;
            OperationByName = operationByName;
        }

        public IDispatchOperationManager OperationManager { get; }

        public IDictionary<string, CancellableOperationDescription> OperationByName { get; }

        public object BeforeCall(string operationName, object[] inputs)
        {
            if (!OperationByName.TryGetValue(operationName, out var description))
            {
                return null;
            }

            var token = description.GetToken(inputs);
            var operation = OperationManager.BeforeCall(description.FullName, token);

            // fix expected type
            description.PassTokenIntoService(inputs, operation?.Token ?? token);

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