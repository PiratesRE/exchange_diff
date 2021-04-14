using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal abstract class BaseObject : IDisposeTrackable, IDisposable
	{
		protected BaseObject()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		protected virtual void InternalDispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
		}

		protected void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString() + " has already been disposed.");
			}
		}

		protected bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose();
				GC.SuppressFinalize(this);
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

		private bool isDisposed;

		private DisposeTracker disposeTracker;
	}
}
