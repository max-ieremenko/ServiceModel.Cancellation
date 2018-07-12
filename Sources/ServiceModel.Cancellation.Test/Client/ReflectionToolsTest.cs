using System;
using NUnit.Framework;

namespace ServiceModel.Cancellation.Client
{
    [TestFixture]
    public class ReflectionToolsTest
    {
        [Test]
        [TestCase("namespace.className.methodName, assemblyName", "namespace.className, assemblyName", "methodName")]
        [TestCase("className.methodName, assemblyName, Culture=neutral, PublicKeyToken=", "className, assemblyName, Culture=neutral, PublicKeyToken=", "methodName")]
        [TestCase("className.methodName", "className", "methodName")]
        [TestCase("className+nested.methodName", "className+nested", "methodName")]
        public void ParseMethodName(string input, string expectedDeclaringTypeName, string expectedMethodName)
        {
            ReflectionTools.ParseMethodName(input, out var declaringTypeName, out var methodName);

            Assert.AreEqual(expectedDeclaringTypeName, declaringTypeName);
            Assert.AreEqual(expectedMethodName, methodName);
        }

        [Test]
        [TestCase("methodName")]
        [TestCase("methodName, assemblyName")]
        [TestCase("methodName,")]
        [TestCase(",methodName")]
        [TestCase(", methodName")]
        [TestCase("methodName, ")]
        public void ParseMethodNameFail(string input)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ReflectionTools.ParseMethodName(input, out _, out _));
        }
    }
}
