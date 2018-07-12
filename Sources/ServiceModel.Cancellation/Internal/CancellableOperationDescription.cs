using System;
using System.Threading;

namespace ServiceModel.Cancellation.Internal
{
    internal struct CancellableOperationDescription
    {
        public CancellableOperationDescription(int tokenArgIndex, Type tokenType, string fullName)
        {
            TokenArgIndex = tokenArgIndex;
            TokenType = tokenType;
            FullName = fullName;
        }

        public int TokenArgIndex { get; }

        public Type TokenType { get; }

        public string FullName { get; }

        internal void PassTokenIntoChannel(object[] args, CancellationTokenProxy? token)
        {
            args[TokenArgIndex] = token;
        }

        internal void PassTokenIntoService(object[] args, CancellationTokenProxy? token)
        {
            var isNullable = Nullable.GetUnderlyingType(TokenType) != null;
            var isToken = TokenType == typeof(CancellationToken) || TokenType == typeof(CancellationToken?);

            if (!token.HasValue)
            {
                if (isNullable)
                {
                    args[TokenArgIndex] = null;
                    return;
                }

                args[TokenArgIndex] = isToken ? (object)default(CancellationToken) : default(CancellationTokenProxy);
                return;
            }

            args[TokenArgIndex] = isToken ? (object)token.Value.Token : token;
        }

        internal CancellationTokenProxy? GetToken(object[] args)
        {
            var value = args[TokenArgIndex];

            if (value == null)
            {
                return null;
            }

            if (value is CancellationTokenProxy proxy)
            {
                return proxy;
            }

            return new CancellationTokenProxy((CancellationToken)value);
        }
    }
}