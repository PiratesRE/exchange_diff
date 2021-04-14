using System;
using System.Threading;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class AsyncResultBase : IAsyncResult
	{
		protected AsyncResultBase(AsyncCallback callback, object state)
		{
			this.callback = callback;
			this.state = state;
			this.thisLock = new object();
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
				if (this.manualResetEvent != null)
				{
					return this.manualResetEvent;
				}
				lock (this.ThisLock)
				{
					if (this.manualResetEvent == null)
					{
						this.manualResetEvent = new ManualResetEvent(this.isCompleted);
					}
				}
				return this.manualResetEvent;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.completedSynchronously;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		private object ThisLock
		{
			get
			{
				return this.thisLock;
			}
		}

		protected void Complete(bool completedSynchronously)
		{
			if (this.isCompleted)
			{
				throw new InvalidOperationException("Cannot call Complete twice");
			}
			this.completedSynchronously = completedSynchronously;
			if (completedSynchronously)
			{
				this.isCompleted = true;
			}
			else
			{
				lock (this.ThisLock)
				{
					this.isCompleted = true;
					if (this.manualResetEvent != null)
					{
						this.manualResetEvent.Set();
					}
				}
			}
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		protected void Complete(bool completedSynchronously, Exception exception)
		{
			this.exception = exception;
			this.Complete(completedSynchronously);
		}

		protected static TAsyncResult End<TAsyncResult>(IAsyncResult result) where TAsyncResult : AsyncResultBase
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			TAsyncResult tasyncResult = result as TAsyncResult;
			if (tasyncResult == null)
			{
				throw new ArgumentException("Invalid async result.", "result");
			}
			if (tasyncResult.endCalled)
			{
				throw new InvalidOperationException("Async object already ended.");
			}
			tasyncResult.endCalled = true;
			if (!tasyncResult.isCompleted)
			{
				tasyncResult.AsyncWaitHandle.WaitOne();
			}
			if (tasyncResult.manualResetEvent != null)
			{
				tasyncResult.manualResetEvent.Close();
			}
			if (tasyncResult.exception != null)
			{
				throw tasyncResult.exception;
			}
			return tasyncResult;
		}

		private AsyncCallback callback;

		private object state;

		private bool completedSynchronously;

		private bool endCalled;

		private Exception exception;

		private bool isCompleted;

		private ManualResetEvent manualResetEvent;

		private object thisLock;
	}
}
