using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using ServiceModel.Cancellation.Client;
using ServiceModel.Cancellation.Service;

namespace ServiceModel.Cancellation.Internal
{
    internal sealed class CancellationEndpointBehavior : IEndpointBehavior
    {
        public CancellationEndpointBehavior(IServiceProvider serviceProvider)
        {
            serviceProvider.IsNotNull(nameof(serviceProvider));

            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Validate(ServiceEndpoint endpoint)
        {
            ContractReader.Validate(endpoint.Contract);
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var operationByName = ContractReader.ExtractCancellableOperations(endpoint.Contract);

            if (operationByName.Count > 0)
            {
                var operationManager = ServiceProvider.Resolve<IDispatchOperationManager>();
                var inspector = new DispatchOperationParameterInspector(operationManager, operationByName);

                foreach (var operation in endpointDispatcher.DispatchRuntime.Operations)
                {
                    if (operationByName.ContainsKey(operation.Name))
                    {
                        operation.ParameterInspectors.Add(inspector);
                    }
                }
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var operationByName = ContractReader.ExtractCancellableOperations(endpoint.Contract);

            if (operationByName.Count > 0)
            {
                var operationManager = ServiceProvider.Resolve<IClientOperationManager>();
                var inspector = new ClientOperationParameterInspector(operationManager, operationByName);

                foreach (var operation in clientRuntime.ClientOperations)
                {
                    if (operationByName.ContainsKey(operation.Name))
                    {
                        operation.ParameterInspectors.Add(inspector);
                    }
                }
            }
        }
    }
}
