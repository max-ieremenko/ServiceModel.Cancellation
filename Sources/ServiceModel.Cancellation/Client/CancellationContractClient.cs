using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace ServiceModel.Cancellation.Client
{
    public class CancellationContractClient : ClientBase<ICancellationContract>, ICancellationContract, IDisposable
    {
        public CancellationContractClient()
        {
        }

        public CancellationContractClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        public CancellationContractClient(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        public CancellationContractClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        public CancellationContractClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        public CancellationContractClient(ServiceEndpoint endpoint)
            : base(endpoint)
        {
        }

        public Task CancelAsync(string operationId)
        {
            return Channel.CancelAsync(operationId);
        }

        void IDisposable.Dispose()
        {
            try
            {
                Close();
            }
            catch
            {
            }
        }
    }
}
