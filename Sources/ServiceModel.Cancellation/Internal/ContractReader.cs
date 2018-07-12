using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Threading;

namespace ServiceModel.Cancellation.Internal
{
    internal static class ContractReader
    {
        public static IDictionary<string, CancellableOperationDescription> ExtractCancellableOperations(ContractDescription contract)
        {
            var indexByOperation = new Dictionary<string, CancellableOperationDescription>(StringComparer.OrdinalIgnoreCase);

            foreach (var operation in contract.Operations)
            {
                var part = operation
                    .Messages
                    .FirstOrDefault(i => i.Direction == MessageDirection.Input)
                    ?.Body
                    .Parts
                    .FirstOrDefault(i => IsTokenOrProxy(i.Type));

                if (part != null)
                {
                    var description = new CancellableOperationDescription(
                        part.Index,
                        part.Type,
                        "{0}.{1}".FormatWith(contract.Name, operation.Name));

                    indexByOperation.Add(operation.Name, description);

                    if (IsToken(part.Type))
                    {
                        operation.KnownTypes.Add(typeof(CancellationTokenProxy?));
                    }
                }
            }

            return indexByOperation;
        }

        public static void Validate(ContractDescription contract)
        {
            foreach (var operation in contract.Operations)
            {
                var operationName = "{0}.{1}".FormatWith(contract.Name, operation.Name);
                var tokensCount = 0;

                foreach (var message in operation.Messages)
                {
                    if (message.Direction == MessageDirection.Output)
                    {
                        ValidateReturnValue(message.Body.ReturnValue.Type, operationName);
                        foreach (var part in message.Body.Parts)
                        {
                            ValidateReturnValue(part.Type, operationName);
                        }
                    }
                    else
                    {
                        foreach (var part in message.Body.Parts)
                        {
                            ValidateArgValue(part.Type, operationName);
                            if (IsTokenOrProxy(part.Type))
                            {
                                tokensCount++;
                            }
                        }
                    }
                }

                if (tokensCount > 1)
                {
                    throw new NotSupportedException(Resources.OperationThereAreDetected2.FormatWith(operationName, tokensCount, nameof(CancellationToken)));
                }
            }
        }

        private static void ValidateReturnValue(Type returnType, string operationName)
        {
            if (IsTokenOrProxy(returnType)
                || IsTokenEnumerable(returnType))
            {
                throw new NotSupportedException(Resources.OperationIsNotSupportedAsReturnArguments1.FormatWith(operationName, nameof(CancellationToken)));
            }
        }

        private static void ValidateArgValue(Type argType, string operationName)
        {
            if (IsTokenEnumerable(argType))
            {
                throw new NotSupportedException(Resources.OperationIsNotSupportedInThisContext.FormatWith(operationName, nameof(CancellationToken)));
            }
        }

        private static bool IsTokenOrProxy(Type type)
        {
            return type == typeof(CancellationTokenProxy)
                   || type == typeof(CancellationTokenProxy?)
                   || IsToken(type);
        }

        private static bool IsToken(Type type)
        {
            return type == typeof(CancellationToken)
                   || type == typeof(CancellationToken?);
        }

        private static bool IsTokenEnumerable(Type type)
        {
            if (IsTokenOrProxy(type))
            {
                return false;
            }

            // IEnumerable<CancellationTokenProxy>, I<x, CancellationTokenProxy?>
            return (type.IsGenericType && type.GetGenericArguments().Any(IsTokenOrProxy))
                   || type == typeof(CancellationTokenProxy[])
                   || type == typeof(CancellationTokenProxy?[])
                   || type == typeof(CancellationToken[])
                   || type == typeof(CancellationToken?[]);
        }
    }
}
