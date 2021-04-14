using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class AsyncResult : DisposeTrackableBase, IAsyncResult
	{
		internal AsyncResult(AsyncCallback callback, object state)
		{
			this.callback = callback;
			this.state = state;
		}

		public void ReportCompletion()
		{
			if (!this.searchCompleteEvent.WaitOne(0))
			{
				this.searchCompleteEvent.Set();
				if (this.callback != null)
				{
					this.callback(this);
				}
			}
		}

		public object AsyncState
		{
			get
			{
				return this.state;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return this.searchCompleteEvent;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.searchCompleteEvent.WaitOne(0, false);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.searchCompleteEvent != null)
			{
				try
				{
					this.searchCompleteEvent.WaitOne();
				}
				finally
				{
					this.searchCompleteEvent.Close();
				}
				this.searchCompleteEvent = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AsyncResult>(this);
		}

		private ManualResetEvent searchCompleteEvent = new ManualResetEvent(false);

		private AsyncCallback callback;

		private object state;
	}
}
