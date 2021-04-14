using System;
using System.Threading;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class LazyAsyncResult : IAsyncResult
	{
		public object ResultObject
		{
			get
			{
				return this.resultObject;
			}
		}

		public object Exception
		{
			get
			{
				return this.exception;
			}
		}

		public LazyAsyncResult(AsyncCallback callback, object state)
		{
			this.callback = callback;
			this.asyncState = state;
		}

		object IAsyncResult.AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.waitHandle == null)
				{
					lock (this.lockObject)
					{
						if (this.waitHandle == null)
						{
							this.waitHandle = new ManualResetEvent(false);
							if (this.isCompleted)
							{
								(this.waitHandle as ManualResetEvent).Set();
							}
						}
					}
				}
				return this.waitHandle;
			}
		}

		bool IAsyncResult.CompletedSynchronously
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
				return this.isCompleted;
			}
		}

		public void Complete(object resultObject, Exception exception)
		{
			this.resultObject = resultObject;
			this.exception = exception;
			this.isCompleted = true;
			if (this.waitHandle != null)
			{
				(this.waitHandle as ManualResetEvent).Set();
				this.waitHandle.Close();
			}
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		private object asyncState;

		private WaitHandle waitHandle;

		private volatile bool isCompleted;

		private object lockObject = new object();

		private AsyncCallback callback;

		private object resultObject;

		private Exception exception;
	}
}
