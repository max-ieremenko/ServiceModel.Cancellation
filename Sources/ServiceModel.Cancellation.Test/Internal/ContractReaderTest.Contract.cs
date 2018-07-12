using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceModel.Cancellation.Internal
{
    public partial class ContractReaderTest
    {
        [ServiceContract]
        private interface ISupported
        {
            [OperationContract]
            Task Method(int arg1, CancellationTokenProxy token, string arg2);

            [OperationContract]
            Task MethodTokenProxy(CancellationTokenProxy token);

            [OperationContract]
            void MethodNullableTokenProxy(CancellationTokenProxy? token);

            [OperationContract]
            void MethodToken(CancellationToken token);

            [OperationContract]
            void MethodNullableToken(CancellationToken? token);
        }

        [ServiceContract]
        private interface IIgnored
        {
            [OperationContract]
            void Method1(int x);
        }

        [ServiceContract]
        private interface IReturnToken
        {
            [OperationContract]
            CancellationTokenProxy Method();
        }

        [ServiceContract]
        private interface ITwoTokens
        {
            [OperationContract]
            void Method(CancellationTokenProxy token1, CancellationTokenProxy token2);
        }

        [ServiceContract]
        private interface ITokensArray
        {
            [OperationContract]
            void Method(CancellationTokenProxy[] tokens);
        }

        [ServiceContract]
        private interface ITokensEnumerable
        {
            [OperationContract]
            void Method(IEnumerable<CancellationTokenProxy> tokens);
        }

        [ServiceContract]
        private interface ITokensCollection
        {
            [OperationContract]
            void Method(ICollection<CancellationTokenProxy> tokens);
        }

        [ServiceContract]
        private interface ITokensList
        {
            [OperationContract]
            void Method(IList<CancellationTokenProxy> tokens);
        }

        [ServiceContract]
        private interface IOutToken
        {
            [OperationContract]
            void Method(out CancellationTokenProxy token);
        }
    }
}