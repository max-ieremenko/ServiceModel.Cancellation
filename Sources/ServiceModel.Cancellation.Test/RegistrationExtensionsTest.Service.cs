using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ServiceModel.Cancellation
{
    public partial class RegistrationExtensionsTest
    {
        [ServiceContract]
        public interface ISomeService
        {
            [OperationContract]
            Task Method(CancellationTokenProxy token);
        }

        public sealed class SomeService : ISomeService
        {
            public Task Method(CancellationTokenProxy token)
            {
                throw new NotImplementedException();
            }
        }
    }
}
