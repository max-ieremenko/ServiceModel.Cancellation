using System;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace ServiceModel.Cancellation.Service
{
    [TestFixture]
    public class DispatchOperationManagerTest
    {
        private Mock<IServiceTokenManager> _manager;
        private DispatchOperationManager _sut;

        [SetUp]
        public void BeforeEachTest()
        {
            _manager = new Mock<IServiceTokenManager>(MockBehavior.Strict);
            _sut = new DispatchOperationManager(_manager.Object);
        }

        [Test]
        public void BeforeCallIgnoreEmptyOperationId()
        {
            var token = new CancellationTokenProxy(CancellationToken.None);

            var operation = _sut.BeforeCall("x", token);
            Assert.IsNull(operation);
        }

        [Test]
        public void BeforeCallBeginOperation()
        {
            var operationId = Guid.NewGuid().ToString();
            var token = new CancellationTokenProxy(CancellationToken.None).NewOperationId(operationId);

            using (var source = new CancellationTokenSource())
            {
                _manager
                    .Setup(m => m.BeginOperation(operationId))
                    .Returns(source.Token);

                var operation = _sut.BeforeCall("x", token);

                _manager.VerifyAll();

                Assert.IsNotNull(operation);
                Assert.AreEqual(operationId, operation.Token.OperationId);
                Assert.AreEqual(source.Token, operation.Token.Token);
            }
        }

        [Test]
        public void AfterCallEndOperation()
        {
            var operationId = Guid.NewGuid().ToString();
            var operation = new OperationInfo
            {
                Token = new CancellationTokenProxy(CancellationToken.None).NewOperationId(operationId)
            };

            _manager.Setup(m => m.EndOperation(operationId));

            _sut.AfterCall(operation);

            _manager.VerifyAll();
        }
    }
}
