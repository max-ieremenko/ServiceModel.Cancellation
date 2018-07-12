using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using NUnit.Framework;
using ServiceModel.Cancellation.Api;

namespace ServiceModel.Cancellation
{
    [TestFixture]
    public class CancellationBehaviorElementTest
    {
        public static ICancellationContract SomeContractFactory()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void DefaultConfiguration()
        {
            var behavior = GetElement("empty");

            var options = behavior.ClientOptions.CreateClientOptions();
            Assert.IsNull(options.ContractFactory);
        }

        [Test]
        public void ClientOptionsConfiguration()
        {
            var behavior = GetElement("clientOptions");

            var options = behavior.ClientOptions.CreateClientOptions();
            Assert.IsNotNull(options.ContractFactory);

            Assert.AreEqual(GetType(), options.ContractFactory.Method.DeclaringType);
            Assert.AreEqual(nameof(SomeContractFactory), options.ContractFactory.Method.Name);
        }

        private CancellationBehaviorElement GetElement(string name)
        {
            EndpointBehaviorElementCollection endpointBehaviors;

            using (var configurationFile = TempFile.FromResources("CancellationBehaviorElementTest.AppConfig.xml"))
            {
                var configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = configurationFile.Location }, ConfigurationUserLevel.None);

                var behaviorsSection = (BehaviorsSection)configuration.GetSection("system.serviceModel/behaviors");
                Assert.IsNotNull(behaviorsSection);

                endpointBehaviors = behaviorsSection.EndpointBehaviors;
            }

            var behavior = endpointBehaviors[name];
            Assert.AreEqual(1, behavior.Count);

            var element = behavior[0];
            Assert.IsInstanceOf<CancellationBehaviorElement>(element);

            return (CancellationBehaviorElement)element;
        }
    }
}