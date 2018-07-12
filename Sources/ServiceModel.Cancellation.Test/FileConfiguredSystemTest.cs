using System;
using System.ServiceModel;
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
    public partial class FileConfiguredSystemTest
    {
        private ServiceHost _userServiceHost;
        private ServiceHost _cancellationServiceHost;
        private Func<UserClient> _userClientFactory;

        [OneTimeSetUp]
        public async Task TestFixtureSetup()
        {
            _cancellationServiceHost = new ServiceHost(typeof(CancellationContractService));

            _userServiceHost = new ServiceHost(typeof(UserService));

            await Task.WhenAll(_userServiceHost.OpenAsync(), _cancellationServiceHost.OpenAsync()).ConfigureAwait(false);

            _userClientFactory = () => new UserClient();
        }

        [OneTimeTearDown]
        public async Task TestFixtureSetupTearDown()
        {
            await Task.WhenAll(_userServiceHost.CloseAsync(), _cancellationServiceHost.CloseAsync()).ConfigureAwait(false);
            ((IDisposable)_userServiceHost).Dispose();
            ((IDisposable)_cancellationServiceHost).Dispose();
        }

        [Test]
        public async Task PingCancellationService()
        {
            using (var client = new CancellationContractClient())
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
            Assert.IsInstanceOf<CombinedServiceProvider>(behavior.ServiceProvider);
        }

        [Test]
        public void ValidateClientConfiguration()
        {
            using (var client = _userClientFactory())
            {
                var behavior = client.Endpoint.Behaviors.Find<CancellationEndpointBehavior>();

                Assert.IsNotNull(behavior);
                Assert.IsInstanceOf<CombinedServiceProvider>(behavior.ServiceProvider);
            }
        }

        [Test]
        public async Task DelayAsyncTokenNone()
        {
            using (var client = _userClientFactory())
            {
                var response = await client.DelayAsync(TimeSpan.Zero, CancellationToken.None).ConfigureAwait(false);

                Assert.IsFalse(response.IsCanceled);
                Assert.IsFalse(response.TokenCanBeCanceled);
            }
        }

        [Test]
        public async Task CancelDelayAsync()
        {
            using (var tokenSource = new CancellationTokenSource())
            using (var client = _userClientFactory())
            {
                tokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));
                var response = await client.DelayAsync(TimeSpan.FromSeconds(10), tokenSource.Token).ConfigureAwait(false);

                Assert.IsTrue(response.IsCanceled);
                Assert.IsTrue(response.TokenCanBeCanceled);
            }
        }
    }
}
