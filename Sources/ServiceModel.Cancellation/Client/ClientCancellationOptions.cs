using System;

namespace ServiceModel.Cancellation.Client
{
    public class ClientCancellationOptions
    {
        public Func<ICancellationContract> ContractFactory { get; set; }
    }
}