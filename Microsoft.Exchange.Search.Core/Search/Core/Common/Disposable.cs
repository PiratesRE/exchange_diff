using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	public abstract class Disposable : IDisposeTrackable, IDisposable
	{
		public Disposable()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		~Disposable()
		{
			this.Dispose(false);
		}

		public bool IsDisposed
		{
			get
			{
				return Interlocked.CompareExchange(ref this.isDisposedFlag, 0, 0) != 0;
			}
		}

		public bool IsDisposing
		{
			get
			{
				return Interlocked.CompareExchange(ref this.isDisposingFlag, 0, 0) != 0;
			}
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
		}

		protected abstract DisposeTracker InternalGetDisposeTracker();

		protected abstract void InternalDispose(bool calledFromDispose);

		protected void CheckDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool calledFromDispose)
		{
			if (Interlocked.Exchange(ref this.isDisposingFlag, 1) == 0)
			{
				try
				{
					if (!this.IsDisposed)
					{
						if (calledFromDispose && this.disposeTracker != null)
						{
							this.disposeTracker.Dispose();
							this.disposeTracker = null;
						}
						this.InternalDispose(calledFromDispose);
						Interlocked.Exchange(ref this.isDisposedFlag, 1);
					}
				}
				finally
				{
					Interlocked.Exchange(ref this.isDisposingFlag, 0);
				}
			}
		}

		private int isDisposedFlag;

		private int isDisposingFlag;

		private DisposeTracker disposeTracker;
	}
}
