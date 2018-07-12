using System;
using System.Linq;

namespace ServiceModel.Cancellation.Internal
{
    internal sealed class CombinedServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider[] _providers;

        public CombinedServiceProvider(params IServiceProvider[] providers)
        {
            _providers = providers;
        }

        public object GetService(Type serviceType)
        {
            return _providers
                .Select(i => i.GetService(serviceType))
                .FirstOrDefault(i => i != null);
        }
    }
}
