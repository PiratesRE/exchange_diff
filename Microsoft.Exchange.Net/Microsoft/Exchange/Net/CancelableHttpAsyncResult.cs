using System;
using System.Threading;

namespace Microsoft.Exchange.Net
{
	internal sealed class CancelableHttpAsyncResult : ICancelableAsyncResult, IAsyncResult
	{
		internal CancelableHttpAsyncResult(CancelableAsyncCallback completionCallback, object state, HttpClient httpClient)
		{
			if (httpClient == null)
			{
				throw new ArgumentNullException("httpClient");
			}
			this.asyncState = state;
			this.httpClient = httpClient;
			this.completionCallback = completionCallback;
		}

		public object AsyncState
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
				lock (this.syncRoot)
				{
					if (this.asyncWaitHandle == null)
					{
						this.asyncWaitHandle = new ManualResetEvent(false);
						if (this.isCompleted)
						{
							this.asyncWaitHandle.Set();
						}
					}
				}
				return this.asyncWaitHandle;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		public bool IsCanceled
		{
			get
			{
				return this.exception is DownloadCanceledException;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.httpClient.CompletedSynchronously;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
			internal set
			{
				this.exception = value;
			}
		}

		internal HttpClient HttpClient
		{
			get
			{
				return this.httpClient;
			}
		}

		internal bool EndCalled
		{
			get
			{
				return this.endCalled;
			}
			set
			{
				this.endCalled = value;
			}
		}

		public void Cancel()
		{
			bool flag = false;
			Exception e = new DownloadCanceledException();
			lock (this.syncRoot)
			{
				if (this.isCompleted)
				{
					return;
				}
				flag = this.httpClient.TryClose(HttpClient.GetDisconnectReason(e));
			}
			if (flag)
			{
				this.InvokeCompleted(e);
			}
		}

		internal void InvokeCompleted()
		{
			lock (this.syncRoot)
			{
				if (this.isCompleted)
				{
					throw new InvalidOperationException("Operation already completed");
				}
				this.isCompleted = true;
				if (this.asyncWaitHandle != null)
				{
					this.asyncWaitHandle.Set();
				}
			}
			if (this.completionCallback != null)
			{
				this.completionCallback(this);
			}
		}

		internal void InvokeCompleted(Exception e)
		{
			this.exception = e;
			this.InvokeCompleted();
		}

		private object syncRoot = new object();

		private object asyncState;

		private HttpClient httpClient;

		private ManualResetEvent asyncWaitHandle;

		private bool isCompleted;

		private CancelableAsyncCallback completionCallback;

		private Exception exception;

		private bool endCalled;
	}
}
