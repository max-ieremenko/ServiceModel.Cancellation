using System.ServiceModel;
using System.Threading.Tasks;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Api
{
    internal static class HostExtensions
    {
        public static Task OpenAsync(this ICommunicationObject channel)
        {
            channel.IsNotNull(nameof(channel));

            return Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, null);
        }

        public static Task CloseAsync(this ICommunicationObject channel)
        {
            channel.IsNotNull(nameof(channel));

            return Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, null);
        }
    }
}