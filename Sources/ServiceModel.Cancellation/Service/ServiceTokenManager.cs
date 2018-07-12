using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Service
{
    internal sealed class ServiceTokenManager : IServiceTokenManager
    {
        private readonly ConcurrentDictionary<string, Operation> _operationById = new ConcurrentDictionary<string, Operation>(StringComparer.OrdinalIgnoreCase);

        public TimeSpan OperationTimeout { get; set; } = TimeSpan.FromHours(3);

        public ILog Log { get; set; }

        public CancellationToken BeginOperation(string operationId)
        {
            operationId.IsNotNullAndNotEmpty(operationId);

            var candidate = new Operation();
            var registered = _operationById.GetOrAdd(operationId, candidate);

            if (registered != candidate)
            {
                candidate.Dispose();

                // created via Cancel
                if (!registered.IsCancellationRequested)
                {
                    // 2 times Begin
                    throw new NotSupportedException(Resources.BeginOperationTwoTimes);
                }
            }

            return registered.Token;
        }

        public void EndOperation(string operationId)
        {
            operationId.IsNotNullAndNotEmpty(operationId);

            if (_operationById.TryRemove(operationId, out var operation))
            {
                operation.Dispose();
                return;
            }

            throw new NotSupportedException(Resources.EndOperationTwoTimes);
        }

        public void CancelOperation(string operationId)
        {
            operationId.IsNotNullAndNotEmpty(operationId);

            var candidate = new Operation(OperationTimeout, t => OnOperationTimeout(operationId, t));
            var registered = _operationById.GetOrAdd(operationId, candidate);

            if (registered != candidate)
            {
                candidate.Dispose();

                // created via Begin
                if (registered.IsCancellationRequested)
                {
                    // 2 times Cancel
                    throw new NotSupportedException(Resources.CancelOperationTwoTimes);
                }
            }

            registered.Cancel();
        }

        private void OnOperationTimeout(string operationId, TimeSpan timeout)
        {
            if (_operationById.TryRemove(operationId, out var operation))
            {
                operation.Dispose();

                if (Log?.IsDebugEnabled == true)
                {
                    Log.Debug(nameof(ServiceTokenManager), "Operation [{0}] timeout after {1}.".FormatWith(operationId, timeout));
                }
            }
        }

        private sealed class Operation : IDisposable
        {
            private readonly Action<TimeSpan> _timeoutCallback;
            private readonly CancellationTokenSource _tokenSource;
            private readonly Timer _timeout;
            private readonly Stopwatch _timeoutTimer;

            public Operation(TimeSpan timeout, Action<TimeSpan> timeoutCallback)
            {
                _timeoutCallback = timeoutCallback;
                _tokenSource = new CancellationTokenSource();
                _timeoutTimer = Stopwatch.StartNew();
                _timeout = new Timer(OnTimeout, null, timeout, TimeSpan.FromMilliseconds(-1));
            }

            public Operation()
            {
                _tokenSource = new CancellationTokenSource();
            }

            public bool IsCancellationRequested => _tokenSource.IsCancellationRequested;

            public CancellationToken Token => _tokenSource.Token;

            public void Dispose()
            {
                _timeout?.Dispose();
                _tokenSource.Dispose();
            }

            public void Cancel() => _tokenSource.Cancel();

            private void OnTimeout(object state)
            {
                _timeout.Dispose();
                _timeoutCallback(_timeoutTimer.Elapsed);
            }
        }
    }
}