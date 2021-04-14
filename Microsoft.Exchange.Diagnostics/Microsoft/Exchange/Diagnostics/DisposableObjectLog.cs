using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class DisposableObjectLog<T> : ObjectLog<T>, IDisposeTrackable, IDisposable
	{
		public DisposableObjectLog(ObjectLogSchema schema, ObjectLogConfiguration configuration) : base(schema, configuration)
		{
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

		public void SuppressDisposeTracker()
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

		protected virtual void InternalDispose(bool disposing)
		{
			base.CloseLog();
		}

		protected virtual DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DisposableObjectLog<T>>(this);
		}

		private bool disposed;

		private DisposeTracker disposeTracker;
	}
}
