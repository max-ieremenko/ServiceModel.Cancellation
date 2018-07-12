using System;
using System.ServiceModel;
using System.Threading.Tasks;
using ServiceModel.Cancellation.Api;

namespace ServiceModel.Cancellation
{
    public partial class FileConfiguredSystemTest
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
            public UserClient()
                : base(nameof(UserClient))
            {
            }

            public Task<DelayResponse> DelayAsync(TimeSpan delay, CancellationTokenProxy token)
            {
                return Channel.DelayAsync(delay, token);
            }
        }

        [ServiceBehavior(ConfigurationName = nameof(UserService))]
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
