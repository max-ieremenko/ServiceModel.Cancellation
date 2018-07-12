using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using ServiceModel.Cancellation.Client;
using ServiceModel.Cancellation.Internal;
using ServiceModel.Cancellation.Service;

namespace ServiceModel.Cancellation
{
    public static class RegistrationExtensions
    {
        public static void UseCancellation<TChannel>(
            this ClientBase<TChannel> client,
            Action<ClientCancellationOptions> configureOptions = null)
            where TChannel : class
        {
            client.IsNotNull(nameof(client));

            RegisterClient(client.Endpoint, configureOptions);
        }

        public static void UseCancellation(
            this ChannelFactory channelFactory,
            Action<ClientCancellationOptions> configureOptions = null)
        {
            channelFactory.IsNotNull(nameof(channelFactory));

            RegisterClient(channelFactory.Endpoint, configureOptions);
        }

        public static ServiceHostBase UseCancellation(this ServiceHostBase host)
        {
            host.IsNotNull(nameof(host));

            UseCancellationAttribute.Register(host.Description);

            return host;
        }

        private static void RegisterClient(ServiceEndpoint endpoint, Action<ClientCancellationOptions> configureOptions)
        {
            if (endpoint.Behaviors.Contains(typeof(CancellationEndpointBehavior)))
            {
                throw new InvalidOperationException(Resources.ServiceEndpointUseCancellationTwice);
            }

            var options = new ClientCancellationOptions();
            configureOptions?.Invoke(options);

            var provider = new ClientServiceProvider { CancellationContractFactory = options.ContractFactory };
            endpoint.Behaviors.Add(new CancellationEndpointBehavior(provider));
        }
    }
}
