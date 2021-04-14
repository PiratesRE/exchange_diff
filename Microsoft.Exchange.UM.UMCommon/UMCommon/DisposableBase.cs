using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class DisposableBase : IDisposeTrackable, IDisposable
	{
		internal DisposableBase()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal bool IsDisposed
		{
			get
			{
				return 1 == Interlocked.CompareExchange(ref this.disposed, 1, 1);
			}
		}

		public void AddReference()
		{
			this.CheckDisposed();
			Interlocked.Increment(ref this.references);
		}

		public void ReleaseReference()
		{
			this.CheckDisposed();
			this.Dispose(true, 1);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public virtual void Dispose()
		{
			this.Dispose(true, Interlocked.CompareExchange(ref this.impliedReference, 0, 1));
		}

		public DisposeTracker GetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
		}

		protected abstract DisposeTracker InternalGetDisposeTracker();

		protected abstract void InternalDispose(bool disposing);

		protected void CheckDisposed()
		{
			if (1 == Interlocked.CompareExchange(ref this.disposed, 1, 1))
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool disposing, int releaseCount)
		{
			if (releaseCount > 0)
			{
				int num = Interlocked.Add(ref this.references, -releaseCount);
				if (num < 0)
				{
					throw new InvalidOperationException();
				}
				if (num == 0)
				{
					if (disposing && this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
					this.InternalDispose(disposing);
					Interlocked.CompareExchange(ref this.disposed, 1, 0);
				}
			}
		}

		private int references = 1;

		private int impliedReference = 1;

		private int disposed;

		private DisposeTracker disposeTracker;

		internal abstract class Finalizable : DisposableBase
		{
			~Finalizable()
			{
				base.Dispose(false, this.references);
			}

			public override void Dispose()
			{
				base.Dispose(true, Interlocked.CompareExchange(ref this.impliedReference, 0, 1));
				GC.SuppressFinalize(this);
			}
		}
	}
}
