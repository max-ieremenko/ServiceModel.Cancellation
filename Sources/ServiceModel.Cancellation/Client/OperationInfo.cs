using System;
using System.Threading.Tasks;

namespace ServiceModel.Cancellation.Client
{
    internal sealed class OperationInfo
    {
        public IDisposable TokenRegistration { get; set; }

        public CancellationTokenProxy Token { get; set; }

        // for tests only
        public Task Cancellation { get; set; }
    }
}