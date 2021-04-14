using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Utils
{
	internal abstract class PushNotificationDisposable : IDisposeTrackable, IDisposable
	{
		public PushNotificationDisposable()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
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

		protected abstract void InternalDispose(bool disposing);

		protected abstract DisposeTracker InternalGetDisposeTracker();

		protected void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				if (disposing && this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				this.InternalDispose(disposing);
			}
		}

		private bool isDisposed;

		private DisposeTracker disposeTracker;
	}
}
