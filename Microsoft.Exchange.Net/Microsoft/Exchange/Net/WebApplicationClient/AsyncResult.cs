using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class AsyncResult : IAsyncResult
	{
		public AsyncResult(AsyncCallback callback, object asyncState)
		{
			this.stopWatch = Stopwatch.StartNew();
			this.callback = callback;
			this.AsyncState = asyncState;
		}

		public void Complete(Exception exception, bool completedSynchronously)
		{
			if (!this.IsCompleted)
			{
				this.stopWatch.Stop();
				this.Exception = exception;
				this.CompletedSynchronously = completedSynchronously;
				this.IsCompleted = true;
				this.asyncWaitHandle.Set();
				if (this.callback != null)
				{
					this.callback(this);
				}
			}
		}

		public Exception Exception { get; private set; }

		public object AsyncState { get; private set; }

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return this.asyncWaitHandle;
			}
		}

		public bool CompletedSynchronously { get; private set; }

		public bool IsCompleted { get; private set; }

		public long ElapsedTicks
		{
			get
			{
				return this.stopWatch.ElapsedTicks;
			}
		}

		private Stopwatch stopWatch;

		private AsyncCallback callback;

		private ManualResetEvent asyncWaitHandle = new ManualResetEvent(false);
	}
}
