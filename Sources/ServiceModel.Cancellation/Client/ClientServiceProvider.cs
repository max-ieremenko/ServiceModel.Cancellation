using System;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Client
{
    internal sealed class ClientServiceProvider : IServiceProvider
    {
        public Func<ICancellationContract> CancellationContractFactory { get; set; }

        public object GetService(Type serviceType)
        {
            serviceType.IsNotNull(nameof(serviceType));

            if (serviceType == typeof(ICancellationContract))
            {
                return ResolveCancellationContract();
            }

            if (serviceType == typeof(Func<ICancellationContract>))
            {
                return ResolveCancellationContractFactory();
            }

            if (serviceType == typeof(IClientOperationManager))
            {
                return ResolveClientOperationManager();
            }

            return null;
        }

        private ICancellationContract ResolveCancellationContract()
        {
            var result = CancellationContractFactory?.Invoke();
            return result ?? new CancellationContractClient();
        }

        private Func<ICancellationContract> ResolveCancellationContractFactory()
        {
            return ResolveCancellationContract;
        }

        private IClientOperationManager ResolveClientOperationManager()
        {
            return new ClientOperationManager(ResolveCancellationContractFactory());
        }
    }
}