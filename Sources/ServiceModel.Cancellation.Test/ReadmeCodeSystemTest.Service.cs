using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using ServiceModel.Cancellation.Api;
using ServiceModel.Cancellation.Service;

namespace ServiceModel.Cancellation
{
    public partial class ReadmeCodeSystemTest
    {
        [ServiceContract]
        public interface IUserContract
        {
            [OperationContract]
            Task<DelayResponse> DelayAsync(TimeSpan delay, CancellationTokenProxy token);
        }

        public class DelayResponse
        {
            public bool IsCanceled { get; set; }

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
        }

        [UseCancellation]
        private sealed class UserService : IUserContract
        {
            public async Task<DelayResponse> DelayAsync(TimeSpan delay, CancellationTokenProxy token)
            {
                try
                {
                    await Task.Delay(delay, token).ConfigureAwait(false);
                }
                catch (TaskCanceledException ex) when (ex.CancellationToken == token)
                {
                }

                return new DelayResponse
                {
                    IsCanceled = token.IsCancellationRequested,
                    TokenCanBeCanceled = token.CanBeCanceled
                };
            }
        }
    }
}
