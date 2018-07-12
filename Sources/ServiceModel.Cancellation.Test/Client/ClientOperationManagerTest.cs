using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace ServiceModel.Cancellation.Client
{
    [TestFixture]
    public partial class ClientOperationManagerTest
    {
        private Mock<IDisposableCancellationContract> _contract;
        private ClientOperationManager _sut;

        [SetUp]
        public void BeforeEachTest()
        {
            _contract = new Mock<IDisposableCancellationContract>(MockBehavior.Strict);

            _sut = new ClientOperationManager(() => _contract.Object);
        }

        [Test]
        public void BeforeCallIgnoreTokenNone()
        {
            var token = new CancellationTokenProxy(CancellationToken.None);

            var operation = _sut.BeforeCall("x", token);
            Assert.IsNull(operation);
        }

        [Test]
        public void BeforeCallIgnoreNullToken()
        {
            var operation = _sut.BeforeCall("x", null);
            Assert.IsNull(operation);
        }

        [Test]
        public void BeforeCallRegisterToken()
        {
            using (var source = new CancellationTokenSource())
            {
                var operation = _sut.BeforeCall("x", new CancellationTokenProxy(source.Token));

                Assert.IsNotNull(operation);
                Assert.AreEqual(source.Token, operation.Token.Token);
                Assert.IsNotNull(operation.Token.OperationId);
                Assert.IsNotNull(operation.TokenRegistration);
            }
        }

        [Test]
        public async Task OnTokenCancellation()
        {
            using (var source = new CancellationTokenSource())
            {
                var operation = _sut.BeforeCall("x", new CancellationTokenProxy(source.Token));
                Assert.IsNotNull(operation);
                Assert.IsNull(operation.Cancellation);

                _contract
                    .Setup(c => c.CancelAsync(operation.Token.OperationId))
                    .Returns(Task.FromResult(true));
                _contract
                    .Setup(c => c.Dispose());

                source.Cancel();

                Assert.IsNotNull(operation.Cancellation);
                await operation.Cancellation.ConfigureAwait(false);

                _contract.VerifyAll();
            }
        }

        [Test]
        public void AfterCallUnRegisterToken()
        {
            var registration = new Mock<IDisposable>(MockBehavior.Strict);
            registration.Setup(r => r.Dispose());

            var operation = new OperationInfo { TokenRegistration = registration.Object };

            _sut.AfterCall(operation);

            registration.VerifyAll();
        }

        [Test]
        public void GenerateOperationId()
        {
            const string OperationFullName = "IService.Method";

            var id1 = ClientOperationManager.GenerateOperationId(OperationFullName);
            var id2 = ClientOperationManager.GenerateOperationId(OperationFullName);

            Console.WriteLine(id1);
            Console.WriteLine(id2);

            StringAssert.Contains(OperationFullName, id1);
            StringAssert.Contains(OperationFullName, id2);
            Assert.AreNotEqual(id1, id2);
        }
    }
}
