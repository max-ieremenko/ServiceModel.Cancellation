using ServiceModel.Cancellation;
using ServiceModel.Cancellation.Client;

namespace Client
{
    public static class CancellationContractClientFactory
    {
        public static ICancellationContract CreateClient()
        {
            return new CancellationContractClient();
        }
    }
}
