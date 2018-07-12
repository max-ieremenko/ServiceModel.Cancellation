using System;
using System.Threading;

namespace ServiceModel.Cancellation
{
    public partial struct CancellationTokenProxy
    {
        public bool IsCancellationRequested => Token.IsCancellationRequested;

        public bool CanBeCanceled => Token.CanBeCanceled;

        public CancellationTokenRegistration Register(Action callback)
        {
            return Token.Register(callback);
        }

        public CancellationTokenRegistration Register(Action callback, bool useSynchronizationContext)
        {
            return Token.Register(callback, useSynchronizationContext);
        }

        public CancellationTokenRegistration Register(Action<object> callback, object state)
        {
            return Token.Register(callback, state);
        }

        public CancellationTokenRegistration Register(
            Action<object> callback,
            object state,
            bool useSynchronizationContext)
        {
            return Token.Register(callback, state, useSynchronizationContext);
        }

        public void ThrowIfCancellationRequested() => Token.ThrowIfCancellationRequested();
    }
}
