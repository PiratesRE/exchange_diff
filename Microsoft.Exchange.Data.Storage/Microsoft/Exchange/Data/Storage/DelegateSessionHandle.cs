using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DelegateSessionHandle : IDisposeTrackable, IDisposable
	{
		internal DelegateSessionHandle(DelegateSessionEntry entry)
		{
			this.entry = entry;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DelegateSessionHandle>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public MailboxSession MailboxSession
		{
			get
			{
				this.CheckDisposed("MailboxSession::get.");
				return this.entry.MailboxSession;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.InternalDispose(disposing);
				this.isDisposed = true;
			}
		}

		private void InternalDispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (disposing)
			{
				if (!this.isDisposed)
				{
					this.entry.DecrementExternalRefCount();
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public const int DelegateSessionCacheCapacity = 6;

		private readonly DelegateSessionEntry entry;

		private bool isDisposed;

		private readonly DisposeTracker disposeTracker;
	}
}
