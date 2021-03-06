﻿using System;
using System.Threading;

namespace devmon_library.Core
{
    internal sealed class Cancellation : ICancellation, IDisposable
    {
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public CancellationToken Token => _cancellationTokenSource.Token;

        public void Cancel() => _cancellationTokenSource.Cancel();

        public void Dispose() => _cancellationTokenSource.Dispose();
    }
}
