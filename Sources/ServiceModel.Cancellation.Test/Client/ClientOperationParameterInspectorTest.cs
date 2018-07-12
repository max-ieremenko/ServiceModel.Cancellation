using System;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using System.Threading;
using Moq;
using NUnit.Framework;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Client
{
    [TestFixture]
    public class ClientOperationParameterInspectorTest
    {
        private CancellableOperationDescription _operation;
        private string _operationName;
        private Mock<IClientOperationManager> _operationManager;
        private IParameterInspector _sut;

        [SetUp]
        public void BeforeEachTest()
        {
            _operationName = Guid.NewGuid().ToString();
            _operation = new CancellableOperationDescription(0, null, "Operation full name");

            var operationByName = new Dictionary<string, CancellableOperationDescription>
            {
                { _operationName, _operation }
            };

            _operationManager = new Mock<IClientOperationManager>(MockBehavior.Strict);
            _sut = new ClientOperationParameterInspector(_operationManager.Object, operationByName);
        }

        [Test]
        public void BeforeCallIgnoreOperation()
        {
            _sut.BeforeCall("unknown", null);
        }

        [Test]
        public void BeforeCallPassTokenIntoChannel()
        {
            using (var sourceInput = new CancellationTokenSource())
            using (var sourceOutput = new CancellationTokenSource())
            {
                var inputToken = new CancellationTokenProxy(sourceInput.Token);
                var outputToken = new CancellationTokenProxy(sourceOutput.Token);

                var inputs = new object[] { inputToken };
                var outputs = new OperationInfo { Token = outputToken };

                _operationManager
                    .Setup(m => m.BeforeCall(_operation.FullName, inputToken))
                    .Returns(outputs);

                var actual = (OperationInfo)_sut.BeforeCall(_operationName, inputs);

                _operationManager.VerifyAll();
                Assert.AreEqual(outputs, actual);
                Assert.AreEqual(outputToken, inputs[0]);
            }
        }

        [Test]
        public void AfterCallIgnoreEmptyOperation()
        {
            _sut.AfterCall(null, null, null, null);
        }

        [Test]
        public void AfterCallInvokeManager()
        {
            var operation = new OperationInfo();
            _operationManager.Setup(m => m.AfterCall(operation));

            _sut.AfterCall(null, null, null, operation);

            _operationManager.VerifyAll();
        }
    }
}
