using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceModel.Cancellation.Api;
using ServiceModel.Cancellation.Client;
using ServiceModel.Cancellation.Internal;
using ServiceModel.Cancellation.Service;

namespace ServiceModel.Cancellation
{
    [TestFixture]
    public partial class ReadmeCodeSystemTest
    {
        private ServiceHost _userServiceHost;
        private ServiceHost _cancellationServiceHost;
        private ChannelFactory<IUserContract> _userChannelFactory;
        private Func<CancellationContractClient> _cancellationClientFactory;

        [OneTimeSetUp]
        public async Task TestFixtureSetup()
        {
            const string UserUrl = "net.tcp://127.0.0.1:2000";
            const string CancellationUrl = "net.tcp://127.0.0.1:2001";

            _cancellationServiceHost = new ServiceHost(typeof(CancellationContractService));
            _cancellationServiceHost.AddServiceEndpoint(typeof(ICancellationContract), new NetTcpBinding(), CancellationUrl);

            _userServiceHost = new ServiceHost(typeof(UserService));
            _userServiceHost.AddServiceEndpoint(typeof(IUserContract), new NetTcpBinding(), UserUrl);

            await Task.WhenAll(_userServiceHost.OpenAsync(), _cancellationServiceHost.OpenAsync()).ConfigureAwait(false);

            _cancellationClientFactory = () => new CancellationContractClient(new NetTcpBinding(), new EndpointAddress(CancellationUrl));

            _userChannelFactory = new ChannelFactory<IUserContract>(new NetTcpBinding(), new EndpointAddress(UserUrl));
            _userChannelFactory.UseCancellation(options =>
            {
                // _
                options.ContractFactory = _cancellationClientFactory;
            });
        }

        [OneTimeTearDown]
        public async Task TestFixtureSetupTearDown()
        {
            await Task.WhenAll(_userServiceHost.CloseAsync(), _cancellationServiceHost.CloseAsync()).ConfigureAwait(false);
            ((IDisposable)_userServiceHost).Dispose();
            ((IDisposable)_cancellationServiceHost).Dispose();
            (_userChannelFactory as IDisposable)?.Dispose();
        }

        [Test]
        public async Task PingCancellationService()
        {
            using (var client = _cancellationClientFactory())
            {
                await client.CancelAsync(Guid.NewGuid().ToString()).ConfigureAwait(false);
            }
        }

        [Test]
        public void ValidateServiceHostConfiguration()
        {
            Assert.AreEqual(1, _userServiceHost.Description.Endpoints.Count);
            var behavior = _userServiceHost.Description.Endpoints[0].Behaviors.Find<CancellationEndpointBehavior>();

            Assert.IsNotNull(behavior);
            Assert.IsInstanceOf<ServiceServiceProvider>(behavior.ServiceProvider);
        }

        [Test]
        public async Task DelayAsyncTokenNone()
        {
            var client = _userChannelFactory.CreateChannel();
            using (client as IDisposable)
            {
                var response = await client.DelayAsync(TimeSpan.Zero, CancellationToken.None).ConfigureAwait(false);

                Assert.IsFalse(response.IsCanceled);
                Assert.IsFalse(response.TokenCanBeCanceled);
            }
        }

        [Test]
        public async Task CancelDelayAsync()
        {
            var client = _userChannelFactory.CreateChannel();
            using (client as IDisposable)
            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));
                var response = await client.DelayAsync(TimeSpan.FromSeconds(10), tokenSource.Token).ConfigureAwait(false);

                Assert.IsTrue(response.IsCanceled);
                Assert.IsTrue(response.TokenCanBeCanceled);
            }
        }
    }
}