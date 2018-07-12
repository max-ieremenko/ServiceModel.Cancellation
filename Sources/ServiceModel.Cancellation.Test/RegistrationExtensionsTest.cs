using System;
using System.ServiceModel;
using NUnit.Framework;

namespace ServiceModel.Cancellation
{
    [TestFixture]
    public partial class RegistrationExtensionsTest
    {
        [Test]
        public void ClientUseCancellationTwice()
        {
            var factory = new ChannelFactory<ISomeService>();
            var behaviorsCount = factory.Endpoint.Behaviors.Count;

            factory.UseCancellation();
            Assert.AreEqual(behaviorsCount + 1, factory.Endpoint.Behaviors.Count);

            Assert.Throws<InvalidOperationException>(() => factory.UseCancellation());
            Assert.AreEqual(behaviorsCount + 1, factory.Endpoint.Behaviors.Count);
        }

        [Test]
        public void ServiceHostUseCancellationTwice()
        {
            var host = new ServiceHost(typeof(SomeService));
            var behaviorsCount = host.Description.Behaviors.Count;

            host.UseCancellation();
            Assert.AreEqual(behaviorsCount + 1, host.Description.Behaviors.Count);

            Assert.Throws<InvalidOperationException>(() => host.UseCancellation());
            Assert.AreEqual(behaviorsCount + 1, host.Description.Behaviors.Count);
        }
    }
}
