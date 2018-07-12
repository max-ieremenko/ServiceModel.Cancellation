using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using ServiceModel.Cancellation.Client;
using ServiceModel.Cancellation.Internal;
using ServiceModel.Cancellation.Service;

namespace ServiceModel.Cancellation
{
    public sealed class CancellationBehaviorElement : BehaviorExtensionElement
    {
        private const string PropertyClientOptions = "clientOptions";

        public override Type BehaviorType => typeof(CancellationEndpointBehavior);

        [ConfigurationProperty(PropertyClientOptions, IsRequired = false)]
        public ClientCancellationOptionsElement ClientOptions => (ClientCancellationOptionsElement)this[PropertyClientOptions];

        protected override object CreateBehavior()
        {
            var clientProvider = new ClientServiceProvider { CancellationContractFactory = null };
            var serviceProvider = new ServiceServiceProvider();

            return new CancellationEndpointBehavior(new CombinedServiceProvider(clientProvider, serviceProvider));
        }
    }
}
