using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using ServiceModel.Cancellation.Api;

namespace ServiceModel.Cancellation
{
    public partial class CodeConfiguredSystemTest
    {
        [ServiceContract]
        public interface IUserContract
        {
            [OperationContract]
            Task<DelayResponse> DelayAsync(TimeSpan delay, CancellationTokenProxy token);

            [OperationContract]
            bool PassCancellationToken(CancellationToken token);

            [OperationContract]
            int PassNullableCancellationToken(CancellationToken? token);

            [OperationContract]
            bool PassCancellationTokenProxy(CancellationTokenProxy token);

            [OperationContract]
            int PassNullableCancellationTokenProxy(CancellationTokenProxy? token);

            ////[OperationContract]
            ////Task<int> PassAsyncCancellationToken(CancellationToken token);

            ////[OperationContract]
            ////Task<int> PassAsyncNullableCancellationToken(CancellationToken? token);
        }

        public class DelayResponse
        {
            public TimeSpan Delay { get; set; }

            public bool IsCanceled { get; set; }

            public bool IsFaulted { get; set; }

            public string ExceptionType { get; set; }

            public string ExceptionMessage { get; set; }

            public bool TokenCanBeCanceled { get; set; }
        }

        private sealed class UserClient : SafeClientBase<IUserContract>, IUserContract
        {
            public UserClient(Binding binding, EndpointAddress remoteAddress)
                : base(binding, remoteAddress)
            {
            }

            public Task<DelayResponse> DelayAsync(TimeSpan delay, CancellationTokenProxy token)
            {
                return Channel.DelayAsync(delay, token);
            }

            public bool PassCancellationToken(CancellationToken token)
            {
                return Channel.PassCancellationToken(token);
            }

            public int PassNullableCancellationToken(CancellationToken? token)
            {
                return Channel.PassNullableCancellationToken(token);
            }

            public bool PassCancellationTokenProxy(CancellationTokenProxy token)
            {
                return Channel.PassCancellationTokenProxy(token);
            }

            public int PassNullableCancellationTokenProxy(CancellationTokenProxy? token)
            {
                return Channel.PassNullableCancellationTokenProxy(token);
            }
        }

        private sealed class UserService : IUserContract
        {
            public static readonly ManualResetEventSlim WaitForOperationStarted = new ManualResetEventSlim(false);

            public async Task<DelayResponse> DelayAsync(TimeSpan delay, CancellationTokenProxy token)
            {
                WaitForOperationStarted.Set();

                var result = new DelayResponse
                {
                    Delay = delay,
                    TokenCanBeCanceled = token.Token.CanBeCanceled
                };

                try
                {
                    await Task.Delay(delay, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    result.IsFaulted = true;
                    result.ExceptionType = ex.GetType().AssemblyQualifiedName;
                    result.ExceptionMessage = ex.ToString();
                }

                result.IsCanceled = token.Token.IsCancellationRequested;
                return result;
            }

            public bool PassCancellationToken(CancellationToken token)
            {
                return token.CanBeCanceled;
            }

            public int PassNullableCancellationToken(CancellationToken? token)
            {
                return RateNullableToken(token);
            }

            public bool PassCancellationTokenProxy(CancellationTokenProxy token)
            {
                return token.Token.CanBeCanceled;
            }

            public int PassNullableCancellationTokenProxy(CancellationTokenProxy? token)
            {
                var result = 0;
                if (token.HasValue)
                {
                    result++;

                    if (token.Value.Token.CanBeCanceled)
                    {
                        result++;
                    }
                }

                return result;
            }

            private static int RateNullableToken(CancellationToken? token)
            {
                var result = 0;
                if (token.HasValue)
                {
                    result++;

                    if (token.Value.CanBeCanceled)
                    {
                        result++;
                    }
                }

                return result;
            }
        }
    }
}
