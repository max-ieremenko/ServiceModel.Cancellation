using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceModel.Cancellation.Api;
using ServiceModel.Cancellation.Client;
using ServiceModel.Cancellation.Service;

namespace ServiceModel.Cancellation
{
    [TestFixture]
    public partial class CodeConfiguredSystemTest
    {
        private ServiceHost _userServiceHost;
        private ServiceHost _cancellationServiceHost;
        private Func<UserClient> _userClientFactory;
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
            _userServiceHost.UseCancellation();

            await Task.WhenAll(_userServiceHost.OpenAsync(), _cancellationServiceHost.OpenAsync()).ConfigureAwait(false);

            _cancellationClientFactory = () => new CancellationContractClient(new NetTcpBinding(), new EndpointAddress(CancellationUrl));

            _userClientFactory = () =>
            {
                var client = new UserClient(new NetTcpBinding(), new EndpointAddress(UserUrl));
                client.UseCancellation(options =>
                {
                    // _
                    options.ContractFactory = _cancellationClientFactory;
                });

                return client;
            };
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
            using (var client = _cancellationClientFactory())
            {
                await client.CancelAsync(Guid.NewGuid().ToString()).ConfigureAwait(false);
            }
        }

        [Test]
        public async Task DelayAsyncTwoParallelNonCancelableActions()
        {
            using (var client = _userClientFactory())
            {
                var t1 = client.DelayAsync(TimeSpan.FromSeconds(.4), CancellationToken.None);
                var t2 = client.DelayAsync(TimeSpan.FromSeconds(.5), CancellationToken.None);

                await Task.WhenAll(t1, t2).ConfigureAwait(false);

                Assert.IsFalse(t1.Result.TokenCanBeCanceled);
                Assert.AreEqual(TimeSpan.FromSeconds(.4), t1.Result.Delay);
                Assert.IsFalse(t1.Result.IsCanceled);
                Assert.IsFalse(t1.Result.IsFaulted);

                Assert.IsFalse(t2.Result.TokenCanBeCanceled);
                Assert.AreEqual(TimeSpan.FromSeconds(.5), t2.Result.Delay);
                Assert.IsFalse(t2.Result.IsCanceled);
                Assert.IsFalse(t2.Result.IsFaulted);
            }
        }

        [Test]
        public async Task DelayAsyncTwoParallelCancelableActions()
        {
            using (var tokenSource = new CancellationTokenSource())
            using (var client = _userClientFactory())
            {
                var t1 = client.DelayAsync(TimeSpan.FromSeconds(.4), tokenSource.Token);
                var t2 = client.DelayAsync(TimeSpan.FromSeconds(.5), tokenSource.Token);

                await Task.WhenAll(t1, t2).ConfigureAwait(false);

                Assert.IsTrue(t1.Result.TokenCanBeCanceled);
                Assert.AreEqual(TimeSpan.FromSeconds(.4), t1.Result.Delay);
                Assert.IsFalse(t1.Result.IsCanceled);
                Assert.IsFalse(t1.Result.IsFaulted);

                Assert.IsTrue(t2.Result.TokenCanBeCanceled);
                Assert.AreEqual(TimeSpan.FromSeconds(.5), t2.Result.Delay);
                Assert.IsFalse(t2.Result.IsCanceled);
                Assert.IsFalse(t2.Result.IsFaulted);
            }
        }

        [Test]
        public async Task CancelDelayAsync()
        {
            using (var tokenSource = new CancellationTokenSource())
            using (var client = _userClientFactory())
            {
                UserService.WaitForOperationStarted.Reset();

                var task = client.DelayAsync(TimeSpan.FromSeconds(3), tokenSource.Token);

                Assert.IsTrue(UserService.WaitForOperationStarted.Wait(TimeSpan.FromSeconds(3)));

                tokenSource.Cancel();

                await task.ConfigureAwait(false);

                Assert.AreEqual(TimeSpan.FromSeconds(3), task.Result.Delay);
                Assert.IsTrue(task.Result.IsCanceled);
                Assert.IsTrue(task.Result.IsFaulted);
                Assert.AreEqual(typeof(TaskCanceledException), Type.GetType(task.Result.ExceptionType, true, false));
            }
        }

        [Test]
        public void PassCancellationToken()
        {
            using (var tokenSource = new CancellationTokenSource())
            using (var client = _userClientFactory())
            {
                Assert.IsFalse(client.PassCancellationToken(CancellationToken.None));
                Assert.IsTrue(client.PassCancellationToken(tokenSource.Token));
            }
        }

        [Test]
        public void PassNullableCancellationToken()
        {
            using (var tokenSource = new CancellationTokenSource())
            using (var client = _userClientFactory())
            {
                Assert.AreEqual(0, client.PassNullableCancellationToken(null));
                Assert.AreEqual(1, client.PassNullableCancellationToken(CancellationToken.None));
                Assert.AreEqual(2, client.PassNullableCancellationToken(tokenSource.Token));
            }
        }

        [Test]
        public void PassCancellationTokenProxy()
        {
            using (var tokenSource = new CancellationTokenSource())
            using (var client = _userClientFactory())
            {
                Assert.IsFalse(client.PassCancellationTokenProxy(CancellationToken.None));
                Assert.IsTrue(client.PassCancellationTokenProxy(tokenSource.Token));
            }
        }

        [Test]
        public void PassNullableCancellationTokenProxy()
        {
            using (var tokenSource = new CancellationTokenSource())
            using (var client = _userClientFactory())
            {
                Assert.AreEqual(0, client.PassNullableCancellationTokenProxy(null));
                Assert.AreEqual(1, client.PassNullableCancellationTokenProxy(CancellationToken.None));
                Assert.AreEqual(2, client.PassNullableCancellationTokenProxy(tokenSource.Token));
            }
        }
    }
}