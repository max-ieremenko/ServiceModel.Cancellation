using System.Threading.Tasks;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Service
{
    public sealed class CancellationContractService : ICancellationContract
    {
        public CancellationContractService()
            : this(ServiceServiceProvider.DefaultTokenManager)
        {
        }

        internal CancellationContractService(IServiceTokenManager tokenManager)
        {
            tokenManager.IsNotNull(nameof(tokenManager));

            TokenManager = tokenManager;
        }

        internal IServiceTokenManager TokenManager { get; }

        public Task CancelAsync(string operationId)
        {
            if (!string.IsNullOrEmpty(operationId))
            {
                TokenManager.CancelOperation(operationId);
            }

            return Task.FromResult((object)null);
        }
    }
}
