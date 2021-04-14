using System;

namespace Microsoft.Exchange.Diagnostics
{
	[CLSCompliant(true)]
	public abstract class DisposeTrackableBase : IForceReportDisposeTrackable, IDisposeTrackable, IDisposable
	{
		public DisposeTrackableBase()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		public DisposeTracker DisposeTracker
		{
			get
			{
				return this.disposeTracker;
			}
		}

		public virtual void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void ForceLeakReport()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.AddExtraDataWithStackTrace("Force leak was called");
			}
			this.disposeTracker = null;
		}

		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
		}

		protected abstract DisposeTracker InternalGetDisposeTracker();

		protected abstract void InternalDispose(bool disposing);

		protected void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		protected void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				this.InternalDispose(disposing);
				this.disposed = true;
			}
		}

		private bool disposed;

		private DisposeTracker disposeTracker;
	}
}
