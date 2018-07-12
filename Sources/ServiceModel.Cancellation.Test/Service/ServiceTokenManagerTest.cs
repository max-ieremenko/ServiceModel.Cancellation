using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace ServiceModel.Cancellation.Service
{
    [TestFixture]
    public class ServiceTokenManagerTest
    {
        private Mock<ILog> _log;
        private ServiceTokenManager _sut;

        [SetUp]
        public void BeforeEachTest()
        {
            _log = new Mock<ILog>(MockBehavior.Strict);
            _log.SetupGet(l => l.IsDebugEnabled).Returns(false);
            _log.SetupGet(l => l.IsInfoEnabled).Returns(false);
            _log.SetupGet(l => l.IsWarningEnabled).Returns(false);
            _log.SetupGet(l => l.IsErrorEnabled).Returns(false);

            _sut = new ServiceTokenManager();
            _sut.Log = _log.Object;
        }

        [Test]
        public void BeginOperationTwoTimes()
        {
            var token = _sut.BeginOperation("1");
            Assert.AreNotEqual(CancellationToken.None, token);
            Assert.IsTrue(token.CanBeCanceled);

            Assert.Throws<NotSupportedException>(() => _sut.BeginOperation("1"));
        }

        [Test]
        public void EndOperationTwoTimes()
        {
            var token = _sut.BeginOperation("1");

            _sut.EndOperation("1");
            Assert.IsFalse(token.IsCancellationRequested);
            Assert.Throws<ObjectDisposedException>(() => Console.Write(token.WaitHandle));

            Assert.Throws<NotSupportedException>(() => _sut.EndOperation("1"));
        }

        [Test]
        public void CancelOperationTwoTimes()
        {
            var token = _sut.BeginOperation("1");

            _sut.CancelOperation("1");
            Assert.IsTrue(token.IsCancellationRequested);
            Assert.IsNotNull(token.WaitHandle);

            Assert.Throws<NotSupportedException>(() => _sut.CancelOperation("1"));
        }

        [Test]
        public void BeginTwoOperations()
        {
            var token1 = _sut.BeginOperation("1");
            var token2 = _sut.BeginOperation("2");

            Assert.AreNotEqual(token1, token2);
        }

        [Test]
        public void BeginEndOperation()
        {
            var token = _sut.BeginOperation("1");

            Assert.IsTrue(token.CanBeCanceled);
            Assert.IsFalse(token.IsCancellationRequested);

            _sut.EndOperation("1");
        }

        [Test]
        public void BeginCancelEndOperation()
        {
            var token = _sut.BeginOperation("1");
            Assert.IsFalse(token.IsCancellationRequested);

            _sut.CancelOperation("1");
            Assert.IsTrue(token.IsCancellationRequested);

            _sut.EndOperation("1");
        }

        [Test]
        public void CancelBeginEndOperation()
        {
            _sut.CancelOperation("1");

            var token = _sut.BeginOperation("1");
            Assert.IsTrue(token.IsCancellationRequested);

            _sut.EndOperation("1");
        }

        [Test]
        public void CancelAlreadyFinishedOperation()
        {
            _sut.OperationTimeout = TimeSpan.FromMilliseconds(500);
            _log.SetupGet(l => l.IsDebugEnabled).Returns(true);

            const string OperationId = "[the operation id]";

            var completion = new TaskCompletionSource<object>();
            _log
                .Setup(l => l.Debug(nameof(ServiceTokenManager), It.IsAny<string>()))
                .Callback<string, string>((source, message) =>
                {
                    Console.WriteLine("{0}: {1}", source, message);
                    StringAssert.Contains(OperationId, message);
                    completion.SetResult(null);
                });

            // cancel request was delivered after EndOperation
            _sut.CancelOperation(OperationId);

            Assert.IsTrue(completion.Task.Wait(TimeSpan.FromSeconds(2)));
        }
    }
}