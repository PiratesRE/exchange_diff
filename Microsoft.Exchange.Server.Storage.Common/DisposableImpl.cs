using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public struct DisposableImpl<T> where T : class, IDisposableImpl
	{
		public DisposableImpl(T disposable)
		{
			this.disposeTracker = disposable.InternalGetDisposeTracker();
		}

		public bool IsDisposed
		{
			get
			{
				return object.ReferenceEquals(this.disposeTracker, DisposableImpl<T>.Disposed);
			}
		}

		public bool IsDisposing
		{
			get
			{
				return object.ReferenceEquals(this.disposeTracker, DisposableImpl<T>.Disposing);
			}
		}

		public void DisposeImpl(T disposable)
		{
			this.Dispose(disposable, true);
			GC.SuppressFinalize(disposable);
		}

		public void FinalizeImpl(T disposable)
		{
			this.Dispose(disposable, false);
		}

		public void SuppressTracking()
		{
			IDisposable disposable = this.disposeTracker;
			if (disposable != null && !object.ReferenceEquals(disposable, DisposableImpl<T>.Disposing) && !object.ReferenceEquals(disposable, DisposableImpl<T>.Disposed))
			{
				((DisposeTracker)disposable).Suppress();
			}
		}

		public void CheckDisposed(T disposable)
		{
			if (object.ReferenceEquals(this.disposeTracker, DisposableImpl<T>.Disposed))
			{
				throw new ObjectDisposedException(disposable.GetType().ToString());
			}
		}

		private void Dispose(T disposable, bool calledFromDispose)
		{
			IDisposable disposable2 = Interlocked.Exchange<IDisposable>(ref this.disposeTracker, DisposableImpl<T>.Disposing);
			if (!object.ReferenceEquals(disposable2, DisposableImpl<T>.Disposing))
			{
				try
				{
					if (!object.ReferenceEquals(disposable2, DisposableImpl<T>.Disposed))
					{
						if (calledFromDispose && disposable2 != null)
						{
							disposable2.Dispose();
							disposable2 = null;
						}
						disposable.InternalDispose(calledFromDispose);
						disposable2 = DisposableImpl<T>.Disposed;
					}
				}
				finally
				{
					Interlocked.Exchange<IDisposable>(ref this.disposeTracker, disposable2);
				}
			}
		}

		private static readonly IDisposable Disposing = new DisposableImpl<T>.FakeFlag();

		private static readonly IDisposable Disposed = new DisposableImpl<T>.FakeFlag();

		private IDisposable disposeTracker;

		private class FakeFlag : IDisposable
		{
			public void Dispose()
			{
			}
		}
	}
}
