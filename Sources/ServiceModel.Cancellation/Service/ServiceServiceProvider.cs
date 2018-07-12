using System;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Service
{
    internal sealed class ServiceServiceProvider : IServiceProvider
    {
        public static IServiceTokenManager DefaultTokenManager { get; } = new ServiceTokenManager();

        public object GetService(Type serviceType)
        {
            serviceType.IsNotNull(nameof(serviceType));

            if (serviceType == typeof(IServiceTokenManager))
            {
                return ResolveServiceTokenManager();
            }

            if (serviceType == typeof(IDispatchOperationManager))
            {
                return ResolveDispatchOperationManager();
            }

            return null;
        }

        private IServiceTokenManager ResolveServiceTokenManager()
        {
            return DefaultTokenManager;
        }

        private IDispatchOperationManager ResolveDispatchOperationManager()
        {
            return new DispatchOperationManager(ResolveServiceTokenManager());
        }
    }
}