using System;

namespace ServiceModel.Cancellation.Client
{
    public partial class ClientOperationManagerTest
    {
        public interface IDisposableCancellationContract : ICancellationContract, IDisposable
        {
        }
    }
}
