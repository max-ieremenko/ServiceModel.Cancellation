using System;
using System.ServiceModel;
using System.Threading.Tasks;
using ServiceModel.Cancellation;

namespace Service
{
    [ServiceContract]
    public interface IDemoService
    {
        [OperationContract]
        Task<OperationResult> RunOperationAsync(TimeSpan delay, CancellationTokenProxy token);
    }

    public class OperationResult
    {
        public TimeSpan ExecutionTime { get; set; }

        public bool IsCanceled { get; set; }
    }
}
