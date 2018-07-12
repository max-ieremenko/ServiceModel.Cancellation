using System;
using System.ServiceModel.Description;
using NUnit.Framework;

namespace ServiceModel.Cancellation.Internal
{
    [TestFixture]
    public partial class ContractReaderTest
    {
        [Test]
        public void ExtractOperationSupported()
        {
            var contract = ContractDescription.GetContract(typeof(ISupported));
            var operations = ContractReader.ExtractCancellableOperations(contract);

            CollectionAssert.AreEqual(
                new[]
                {
                    nameof(ISupported.Method),
                    nameof(ISupported.MethodTokenProxy),
                    nameof(ISupported.MethodNullableTokenProxy),
                    nameof(ISupported.MethodToken),
                    nameof(ISupported.MethodNullableToken)
                },
                operations.Keys);

            var operation = operations[nameof(ISupported.Method)];
            Assert.AreEqual(1, operation.TokenArgIndex);
            Assert.AreEqual("ISupported.Method", operation.FullName);

            operation = operations[nameof(ISupported.MethodTokenProxy)];
            Assert.AreEqual(0, operation.TokenArgIndex);
            Assert.AreEqual("ISupported.MethodTokenProxy", operation.FullName);

            operation = operations[nameof(ISupported.MethodNullableTokenProxy)];
            Assert.AreEqual(0, operation.TokenArgIndex);
            Assert.AreEqual("ISupported.MethodNullableTokenProxy", operation.FullName);

            operation = operations[nameof(ISupported.MethodToken)];
            Assert.AreEqual(0, operation.TokenArgIndex);
            Assert.AreEqual("ISupported.MethodToken", operation.FullName);

            operation = operations[nameof(ISupported.MethodNullableToken)];
            Assert.AreEqual(0, operation.TokenArgIndex);
            Assert.AreEqual("ISupported.MethodNullableToken", operation.FullName);
        }

        [Test]
        public void ExtractOperationIgnored()
        {
            var contract = ContractDescription.GetContract(typeof(IIgnored));

            var operations = ContractReader.ExtractCancellableOperations(contract);
            CollectionAssert.IsEmpty(operations.Keys);
        }

        [Test]
        [TestCase(typeof(ISupported))]
        [TestCase(typeof(IIgnored))]
        public void ValidatePass(Type contractType)
        {
            var contract = ContractDescription.GetContract(contractType);

            ContractReader.Validate(contract);
        }

        [Test]
        [TestCase(typeof(IReturnToken))]
        [TestCase(typeof(ITwoTokens))]
        [TestCase(typeof(ITokensArray))]
        [TestCase(typeof(ITokensEnumerable))]
        [TestCase(typeof(ITokensCollection))]
        [TestCase(typeof(ITokensList))]
        [TestCase(typeof(IOutToken))]
        public void ValidateFail(Type contractType)
        {
            var contract = ContractDescription.GetContract(contractType);

            var ex = Assert.Throws<NotSupportedException>(() => ContractReader.Validate(contract));
            Console.WriteLine(ex.Message);
        }
    }
}
