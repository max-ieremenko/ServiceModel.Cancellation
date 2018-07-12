using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

namespace ServiceModel.Cancellation
{
    [Serializable]
    [DebuggerDisplay("Id = {OperationId}; IsCancellationRequested = {Token.IsCancellationRequested}")]
    public partial struct CancellationTokenProxy : ISerializable, IEquatable<CancellationTokenProxy>, IEquatable<CancellationToken>
    {
        public CancellationTokenProxy(CancellationToken token)
            : this(token, null)
        {
            Token = token;
            OperationId = null;
        }

        private CancellationTokenProxy(CancellationToken token, string operationId)
        {
            Token = token;
            OperationId = operationId;
        }

        private CancellationTokenProxy(SerializationInfo info, StreamingContext context)
        {
            Token = CancellationToken.None;
            OperationId = info.GetString(nameof(OperationId));
        }

        public CancellationToken Token { get; }

        internal string OperationId { get; }

        public static implicit operator CancellationTokenProxy(CancellationToken token)
        {
            return new CancellationTokenProxy(token);
        }

        public static implicit operator CancellationToken(CancellationTokenProxy token)
        {
            return token.Token;
        }

        public static bool operator ==(CancellationTokenProxy left, CancellationToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CancellationTokenProxy left, CancellationToken right)
        {
            return !left.Equals(right);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(OperationId), OperationId);
        }

        public bool Equals(CancellationTokenProxy other)
        {
            return Token.Equals(other.Token);
        }

        public bool Equals(CancellationToken other)
        {
            return Token.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj is CancellationTokenProxy token && Equals(token);
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }

        internal CancellationTokenProxy NewToken(CancellationToken token)
        {
            return new CancellationTokenProxy(token, OperationId);
        }

        internal CancellationTokenProxy NewOperationId(string operationId)
        {
            return new CancellationTokenProxy(Token, operationId);
        }
    }
}