using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class DisposableObject : IDisposeTrackable, IDisposable
	{
		protected DisposableObject()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
		}

		protected virtual void CheckDisposed(string methodName = null)
		{
			if (this.isDisposed)
			{
				if (methodName == null)
				{
					methodName = new StackTrace(1).GetFrame(0).GetMethod().Name;
				}
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().FullName + " has already been disposed.");
			}
		}

		public DisposeTracker DisposeTracker
		{
			get
			{
				return this.disposeTracker;
			}
		}

		protected bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return this.GetDisposeTracker();
		}

		protected abstract DisposeTracker GetDisposeTracker();

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private readonly DisposeTracker disposeTracker;

		private bool isDisposed;
	}
}
