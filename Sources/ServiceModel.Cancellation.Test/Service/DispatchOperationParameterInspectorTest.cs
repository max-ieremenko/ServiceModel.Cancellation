using System;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using System.Threading;
using Moq;
using NUnit.Framework;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Service
{
    [TestFixture]
    public class DispatchOperationParameterInspectorTest
    {
        private Mock<IDispatchOperationManager> _manager;
        private CancellableOperationDescription _operation;
        private string _operationName;
        private IParameterInspector _sut;

        [SetUp]
        public void BeforeEachTest()
        {
            _operationName = Guid.NewGuid().ToString();
            _operation = new CancellableOperationDescription(0, typeof(CancellationTokenProxy), "Operation full name");

            var operationByName = new Dictionary<string, CancellableOperationDescription>
            {
                { _operationName, _operation }
            };

            _manager = new Mock<IDispatchOperationManager>(MockBehavior.Strict);
            _sut = new DispatchOperationParameterInspector(_manager.Object, operationByName);
        }

        [Test]
        public void BeforeCallIgnoreOperation()
        {
            _sut.BeforeCall("unknown", null);
        }

        [Test]
        public void BeforeCallFixExpectedType()
        {
            using (var source = new CancellationTokenSource())
            {
                var token = new CancellationTokenProxy(source.Token);
                var inputs = new object[] { token };

                _manager
                    .Setup(m => m.BeforeCall(_operation.FullName, token))
                    .Returns((OperationInfo)null);

                var actual = _sut.BeforeCall(_operationName, inputs);

                _manager.VerifyAll();

                Assert.IsNull(actual);
                Assert.AreEqual(token, inputs[0]);
            }
        }

        [Test]
        public void BeforeCallChangeToken()
        {
            using (var source = new CancellationTokenSource())
            {
                var token = new CancellationTokenProxy(source.Token);
                var inputs = new object[1];
                var expected = new OperationInfo { Token = token };

                _manager
                    .Setup(m => m.BeforeCall(_operation.FullName, null))
                    .Returns(expected);

                var actual = _sut.BeforeCall(_operationName, inputs);

                _manager.VerifyAll();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(token, inputs[0]);
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
            _manager
                .Setup(m => m.AfterCall(operation));

            _sut.AfterCall(null, null, null, operation);

            _manager.VerifyAll();
        }
    }
}