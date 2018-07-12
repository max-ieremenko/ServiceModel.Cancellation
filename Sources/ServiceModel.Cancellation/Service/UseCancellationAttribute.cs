using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Service
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UseCancellationAttribute : Attribute, IServiceBehavior
    {
        private IEndpointBehavior _endpointBehavior;

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                GetEndpointBehavior().Validate(endpoint);
            }
        }

        void IServiceBehavior.AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                if (!endpoint.Behaviors.Contains(GetEndpointBehavior().GetType()))
                {
                    endpoint.Behaviors.Add(GetEndpointBehavior());
                }
            }
        }

        internal static void Register(ServiceDescription serviceDescription)
        {
            if (serviceDescription.Behaviors.Contains(typeof(UseCancellationAttribute)))
            {
                throw new InvalidOperationException(Resources.ServiceHostUseCancellationTwice);
            }

            serviceDescription.Behaviors.Add(new UseCancellationAttribute());
        }

        private IEndpointBehavior GetEndpointBehavior()
        {
            if (_endpointBehavior == null)
            {
                _endpointBehavior = new CancellationEndpointBehavior(new ServiceServiceProvider());
            }

            return _endpointBehavior;
        }
    }
}
