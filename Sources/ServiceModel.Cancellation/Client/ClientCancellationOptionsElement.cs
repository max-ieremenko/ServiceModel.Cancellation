using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Client
{
    public class ClientCancellationOptionsElement : ConfigurationElement
    {
        private const string PropertyContractFactory = "contractFactory";

        [ConfigurationProperty(PropertyContractFactory, IsRequired = false)]
        public string ContractFactory
        {
            get => (string)this[PropertyContractFactory];
            set => this[PropertyContractFactory] = value;
        }

        internal ClientCancellationOptions CreateClientOptions()
        {
            var result = new ClientCancellationOptions();

            try
            {
                result.ContractFactory = ParseContractFactory(ContractFactory);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(Resources.ClientCancellationOptionsElementContractFactory0.FormatWith(ContractFactory), ex);
            }

            return result;
        }

        private static Func<ICancellationContract> ParseContractFactory(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return null;
            }

            // className.methodName, assemblyName
            ReflectionTools.ParseMethodName(fullName, out var declaringTypeName, out var methodName);

            var declaringType = Type.GetType(declaringTypeName, true, false);
            var methods = declaringType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(i => i.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                .Where(i => i.ReturnType == typeof(ICancellationContract))
                .Where(i => i.GetParameters().Length == 0)
                .ToList();

            if (methods.Count == 0)
            {
                throw new ConfigurationErrorsException(Resources.ClientCancellationOptionsElementMethodNotFound1.FormatWith(methodName, declaringTypeName));
            }

            if (methods.Count > 1)
            {
                throw new ConfigurationErrorsException(Resources.ClientCancellationOptionsElementTooManyMethodsFound2.FormatWith(methods.Count, methodName, declaringTypeName));
            }

            return methods[0].AsStatic<Func<ICancellationContract>>();
        }
    }
}
