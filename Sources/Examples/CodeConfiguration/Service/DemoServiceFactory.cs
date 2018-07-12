using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using ServiceModel.Cancellation;

namespace Service
{
    public sealed class DemoServiceFactory : ServiceHostFactoryBase
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var host = new ServiceHost(typeof(DemoService), baseAddresses);

            // !!! ServiceModel.Cancellation demo: set-up host to support cancellation
            host.UseCancellation();

            return host;
        }
    }
}