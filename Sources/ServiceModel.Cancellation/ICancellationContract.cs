using System.ServiceModel;
using System.Threading.Tasks;

namespace ServiceModel.Cancellation
{
    [ServiceContract]
    public interface ICancellationContract
    {
        [OperationContract]
        Task CancelAsync(string operationId);
    }
}
