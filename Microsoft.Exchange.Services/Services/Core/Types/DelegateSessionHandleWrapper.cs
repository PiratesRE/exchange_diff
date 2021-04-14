using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DelegateSessionHandleWrapper : IDisposeTrackable, IDisposable
	{
		public DelegateSessionHandleWrapper(DelegateSessionHandle sessionHandle)
		{
			this.handle = sessionHandle;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DelegateSessionHandle Handle
		{
			get
			{
				return this.handle;
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DelegateSessionHandleWrapper>(this);
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
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
		}

		private void Dispose(bool isDisposing)
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.handle != null)
				{
					this.handle.Dispose();
					this.handle = null;
				}
				if (isDisposing)
				{
					GC.SuppressFinalize(this);
				}
			}
		}

		private readonly DisposeTracker disposeTracker;

		private DelegateSessionHandle handle;

		private bool disposed;
	}
}
