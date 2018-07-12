using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ServiceModel.Cancellation.Api
{
    internal abstract class SafeClientBase<TChannel> : ClientBase<TChannel>, IDisposable
        where TChannel : class
    {
        protected SafeClientBase(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        protected SafeClientBase(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
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
