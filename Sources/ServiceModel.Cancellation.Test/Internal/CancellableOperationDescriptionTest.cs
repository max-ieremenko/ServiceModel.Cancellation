using System;
using System.Threading;
using NUnit.Framework;

namespace ServiceModel.Cancellation.Internal
{
    [TestFixture]
    public class CancellableOperationDescriptionTest
    {
        private static object[] GetTokenCases => new object[]
        {
            new object[] { null, null },
            new object[] { new CancellationTokenProxy(CancellationToken.None), new CancellationTokenProxy(CancellationToken.None) },
            new object[] { CancellationToken.None, new CancellationTokenProxy(CancellationToken.None) },
        };

        // see ClientOperationParameterInspector
        private static object[] PassTokenIntoChannelCases => new object[]
        {
            new object[] { null, null },
            new object[] { new CancellationTokenProxy(CancellationToken.None), new CancellationTokenProxy(CancellationToken.None) },
        };

        private static object[] PassTokenIntoServiceCases => new object[]
        {
            new object[] { typeof(CancellationTokenProxy?), null, null },
            new object[] { typeof(CancellationToken?), null, null },

            new object[] { typeof(CancellationTokenProxy), null, default(CancellationTokenProxy) },
            new object[] { typeof(CancellationToken), null, default(CancellationToken) },

            new object[] { typeof(CancellationTokenProxy), new CancellationTokenProxy(CancellationToken.None), new CancellationTokenProxy(CancellationToken.None) },
            new object[] { typeof(CancellationToken), new CancellationTokenProxy(CancellationToken.None), CancellationToken.None },
        };

        [Test]
        [TestCaseSource(typeof(CancellableOperationDescriptionTest), nameof(GetTokenCases))]
        public void GetToken(object inputValue, object expected)
        {
            var operation = new CancellableOperationDescription(1, null, null);

            var args = new object[3];
            args[1] = inputValue;

            Assert.AreEqual(expected, operation.GetToken(args));
        }

        [Test]
        [TestCaseSource(typeof(CancellableOperationDescriptionTest), nameof(PassTokenIntoChannelCases))]
        public void PassTokenIntoChannel(CancellationTokenProxy? valueToSet, object expected)
        {
            var operation = new CancellableOperationDescription(1, null, null);

            var args = new object[3];

            operation.PassTokenIntoChannel(args, valueToSet);

            Assert.AreEqual(expected, args[1]);
        }

        [Test]
        [TestCaseSource(typeof(CancellableOperationDescriptionTest), nameof(PassTokenIntoServiceCases))]
        public void PassTokenIntoService(Type tokenType, CancellationTokenProxy? valueToSet, object expected)
        {
            var operation = new CancellableOperationDescription(1, tokenType, null);

            var args = new object[3];

            operation.PassTokenIntoService(args, valueToSet);

            Assert.AreEqual(expected, args[1]);
        }
    }
}
